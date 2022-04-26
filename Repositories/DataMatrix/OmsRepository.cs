using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BoxTrackLabel.API.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace BoxTrackLabel.API.Repositories
{
    public class OmsRepository
    {
        //Orders
        public async Task<Tuple<HttpResponseMessage,string>> PostOrder(Order order)
        {
            var productsObject = new []{
            new{
                cisType = order.CisType, gtin = order.Gtin.PadLeft(14,'0'), mrp = order.Mrp, quantity = order.Quantity, serialNumberType = order.SerialNumberType,
                stickerId = order.StickerId??0, templateId = order.TemplateId
            }
            };
            var jsonRequest = JsonConvert.SerializeObject(new {
                contactPerson = order.ContactPerson, createMethodType = order.CreateMethodType, expectedStartDate = order.ExpectedStartDate.Value.ToString("yyyy-MM-dd"),
                factoryAddress = order.FactoryAddress, factoryCountry = order.FactoryCountry, factoryId = order.FactoryId, 
                factoryName = order.FactoryName, poNumber = order.PoNumber??123456, productCode = order.ProductCode, 
                productDescription = order.ProductDescription, productionLineId = order.ProductionLineId, productionOrderId = order.ProductionOrderId==""?"123456":order.ProductionOrderId,
                products = productsObject, releaseMethodType = order.ReleaseMethodType
            }); 
            var body = new StringContent(jsonRequest,Encoding.UTF8,"application/json");
            var postResponse = await PostRequest(order,"/api/v2/otp/orders?omsId="+order.OmsId,body);
            return postResponse;
        }
        public async Task<Tuple<HttpResponseMessage,string>> CheckAvailability(Order order)
        {
            var availabilityResponse = await GetRequest(order,"/api/v2/otp/orders?omsId="+order.OmsId);
            return availabilityResponse;
        }
        public async Task<Tuple<HttpResponseMessage,String>> SendUtilisationReport(Order order)
        {
            var confirmedGtins = order.Codes.Where(c=>c.IsConfirmed == true).Select(c=>c.CodeValue).ToArray();
            var jsonConfirmedRequest = JsonConvert.SerializeObject(new {
                usageType = "USED_FOR_PRODUCTION", sntins = confirmedGtins , productionLineId = order.ProductionLineId
            });
            var body = new StringContent(jsonConfirmedRequest,Encoding.UTF8,"application/json");
            var postResponse = await PostRequest(order,"/api/v2/otp/utilisation?omsId="+order.OmsId,body);
            return Tuple.Create(postResponse.Item1, postResponse.Item2);
        }
        public async Task<Tuple<HttpResponseMessage,String>> SendDropoutReport(Order order)
        {
            var dropoutGtins = order.Codes.Where(c=>c.IsConfirmed == false).Select(c=>c.CodeValue).ToArray();
            var jsonDropoutRequest =  JsonConvert.SerializeObject(new {
                    dropOutReason = "UNUSED", sntins = dropoutGtins , productionLineId = order.ProductionLineId
            });
            var body = new StringContent(jsonDropoutRequest,Encoding.UTF8,"application/json"); 
            var postResponse = await PostRequest(order,"/api/v2/otp/dropout?omsId="+order.OmsId,body);
            return Tuple.Create(postResponse.Item1, postResponse.Item2);
        }
        public async Task<Tuple<HttpResponseMessage,String>> CloseOrderBuffer(Order order)
        {
            var body = new StringContent("",Encoding.UTF8,"application/json");
            var postResponse = await PostRequest(order,"/api/v2/otp/buffer/close?omsId="+order.OmsId+"&orderId="+order.OrderId+"&",body);
            return Tuple.Create(postResponse.Item1, postResponse.Item2);
        }

        //Codes
        public async Task<Tuple<HttpResponseMessage,String>> GetCodes(Order order)
        {
            var codeResponse = await GetRequest(order,"/api/v2/otp/codes?gtin="+order.Gtin.PadLeft(14,'0')+"&omsId="+order.OmsId+"&orderId="+order.OrderId+"&quantity="+order.Quantity);
            return codeResponse;
        }
        public async Task<List<Code>> CodesRetry(Order order, string blockId)
        {
            var codesRetryResponse =  await GetRequest(order,"/api/v2/otp/codes/retry?gtin="+order.Gtin.PadLeft(14,'0')+"&omsId="+order.OmsId+"&orderId="+order.OrderId+"&blockId="+blockId);
            var response = codesRetryResponse.Item1;
            var responseContent = codesRetryResponse.Item2;
            var codeList = new List<Code>();
            if(response.IsSuccessStatusCode)
            {
                var codeResponse = JsonConvert.DeserializeObject<CodeResponse>(responseContent);
                var codes = codeResponse.codes;
                if(codes.Length > 0)
                {
                    foreach (var code in codes)
                    {
                        codeList.Add(
                            new Code{ CodeValue = code, OrderId = order.Id, CisType = order.CisType}
                        );
                    }
                }
            }
            else
            {
                Log.Error("Error durante el reintento de descarga de los códigos datamatrix de la orden: {0}, detalles: {1}", order.OrderId, responseContent);
            }
            return codeList;
        }
        public async Task<List<string>> GetBlockIds(Order order)
        {
            var blockIdResponse = await GetRequest(order,"/api/v2/otp/codes/blocks?gtin="+order.Gtin.PadLeft(14,'0')+"&omsId="+order.OmsId+"&orderId="+order.OrderId);
            var response = blockIdResponse.Item1;
            var responseContent = blockIdResponse.Item2;
            var blockIdList = new List<string>();
            if(response.IsSuccessStatusCode)
            {
                var blockIdResponseObject = JsonConvert.DeserializeObject<BlocksResponse>(responseContent);
                blockIdList = blockIdResponseObject.Blocks.Select(b => b.blockId).ToList();
            }
            else
            {
                Log.Error("Error durante la consulta de los códigos BlockIds de la orden: {0}, detalles: {1}", order.OrderId, responseContent);
            }
            return blockIdList;
        }
        private async Task<Tuple<HttpResponseMessage,string>> GetRequest(Order order, string requestUrl)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(order.OmsUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Add("clienttoken", order.Token); 
                httpClient.Timeout = TimeSpan.FromMinutes(2);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await httpClient.GetAsync(requestUrl);
                var responseContent = await response.Content.ReadAsStringAsync(); 
                return Tuple.Create(response, responseContent);
            }
        }
        private async Task<Tuple<HttpResponseMessage,string>> PostRequest(Order order, string requestUrl, StringContent body)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(order.OmsUrl);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Add("clienttoken", order.Token); 
                httpClient.Timeout = TimeSpan.FromMinutes(2);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await httpClient.PostAsync(requestUrl,body);
                var responseContent = await response.Content.ReadAsStringAsync(); 
                return Tuple.Create(response, responseContent);
            }
        }
        public async Task<string> TranslateThisAsync(dynamic jsonRequest)
        {
            //dynamic jsonRequest = value;

            string translatedText = "";

            var url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl="
            + jsonRequest.sourceLang + "&tl=" + jsonRequest.targetLang + "&dt=t&q=" + jsonRequest.sourceText;

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(url))
            using (HttpContent content = response.Content)
            {
                // ... Read the response as a string.
                var tr = content.ReadAsStringAsync().Result;
                // ... turn to an Jarray to be easier to select
                JArray ja = JsonConvert.DeserializeObject<JArray>(tr);
                // ... read the data we want
                translatedText = ja[0][0][0].ToString();
            }
            return translatedText;
        }
    }
}
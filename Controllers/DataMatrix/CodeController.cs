using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BoxTrackLabel.API.Models;
using BoxTrackLabel.API.Repositories;
using BoxTrackLabel.API.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using Serilog;

namespace BoxTrackLabel.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CodeController : DataMatrixBaseController<Code, CodesRepository>
    {
        private readonly CodesRepository _repository;
        private readonly OmsRepository omsRepository;

        public CodeController(CodesRepository repository, OmsRepository omsRepository) : base(repository)
        {
           _repository = repository;
            this.omsRepository = omsRepository;
        }
        /// <summary>
        /// Método para la requisición de códigos datamatrix de una determinada orden
        /// </summary>
        /// <remarks>
        /// Ejemplo de petición:
        ///
        ///     POST /GetCodes
        ///     {
        ///        "clientToken": "1a866e5a-38d3-447c-a761-50be5fd86316",
        ///        "gtin": 07465603170599,
        ///        "omsId": "34e4a261-9842-46d3-96cc-2c22fab4be05"
        ///        "orderId": "9cbc151a-9ff1-47ad-98d5-e4edd3dda13e"
        ///        "quantity": 52
        ///     }
        ///     Retorna: Un listado de códigos datamatrix
        /// </remarks>
        /// <param name="order">La orden de la cual se requieren los códigos</param>
        /// <response code="200">Cuando se realiza la requisición exitosamente, se obtiene un listado de códigos con la siguiente estructura:</response>
        /// <response code="400">La requisición falló debido algún parámetro erróneo o excepción lanzada</response>
        [Route("GetCodes")] 
        [HttpPost]
        [Authorize(Permissions.Menus.DataMatrix_SUPERVISOR)]
        public async Task<ActionResult<IEnumerable<Code>>> GetCodes(Order order)
        {
            try
            {
                var codeResponse = await omsRepository.GetCodes(order);
                var response = codeResponse.Item1;
                var responseContent = codeResponse.Item2;
                if(response.IsSuccessStatusCode)
                {
                    var codeResponseObject = JsonConvert.DeserializeObject<CodeResponse>(responseContent);
                    var codes = codeResponseObject.codes;
                    if(codes.Length > 0)
                    {
                        var codeList = new List<Code>();
                        foreach (var code in codes)
                        {
                            codeList.Add(
                                new Code{ CodeValue = code, OrderId = order.Id, CisType = order.CisType}
                            );
                        }
                        var codesResult = await _repository.ProcessOrder(codeList);
                        return Ok(codesResult);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Ha ocurrido un error, favor intentar nuevamente");
                        return BadRequest(ModelState);
                    }
                }
                else
                {
                    if(responseContent.Contains("EXHAUSTED"))
                    {
                        return await GetCodesRetry(order);
                    } 
                    var translateRequestObject = new {sourceLang = "ru", targetLang="es", sourceText = responseContent}; 
                    var translatedStringMessage = await omsRepository.TranslateThisAsync(translateRequestObject);
                    Log.Error("Ocurrió un error obteniendo los códigos de la orden {0}, detalles {1}",order.OrderId,translatedStringMessage);
                    var errorMessage = FormatErrorMessage(translatedStringMessage);
                    ModelState.AddModelError("", errorMessage);
                    return BadRequest(ModelState);
                }
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
        }
        /// <summary>
        /// Método para marcar los códigos datamatrix previamente escaneados como confirmados
        /// </summary>
        /// <remarks>
        /// Ejemplo de petición:
        ///
        ///     POST /GetCodes
        ///     {
        ///        "code": "07465603170599vopsxSPAAAAdGVz",
        ///     }
        ///     Retorna: un código de estado 200 acompañado del código confirmado
        /// </remarks>
        /// <param name="code">El código a confirmar</param>
        /// <response code="200">Cuando se realiza la confirmación exitosamente, se obtiene el código de respuesta 200 acompañado del código que fue confirmado:</response>
        /// <response code="400">La confirmación falló debido algún parámetro erróneo o excepción lanzada</response>
        [Route("ConfirmCode")] 
        [HttpPost]
        [Authorize(Permissions.Menus.DataMatrix_OPERADOR)]
        public async Task<ActionResult<IEnumerable<Code>>> ConfirmCode([FromForm]string code)
        {
            try
            {
                var userClaim = HttpContext.User.Claims.Where(c=>c.Type==Values.UserClaimKey).SingleOrDefault();
                string user = userClaim != null ? userClaim.Value : null; 
                var codeResponse = await _repository.ConfirmCode(code, user);
                return Ok(new {responseType = codeResponse.Item1, response = codeResponse.Item2});  
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
        }
        /// <summary>
        /// Método para consultar una orden y verificar si su impresión fue autorizada.
        /// </summary>
        /// <remarks>
        /// Ejemplo de petición:
        ///
        ///     GET /CanPrintOrder/9cbc151a-9ff1-47ad-98d5-e4edd3dda13e
        ///     Retorna: un valor booleano que indica si podemos imprimir los códigos de la orden
        /// </remarks>
        /// <param name="orderId">Clave de la orden a consultar</param>
        /// <response code="200">Se obtiene el código de respuesta 200 acompañado del valor booleano true</response>
        /// <response code="400">No existen códigos disponibles o se ha lanzado un excepción</response>
        [Route("CanPrintOrder/{orderId}")] 
        [HttpGet]
        [Authorize(Permissions.Menus.DataMatrix_OPERADOR)]
        public async Task<ActionResult<bool>> CanPrintOrder(int orderId)
        {
            try
            {
                var availableCodes = await _repository.getAvailableOrderCodes(orderId);
                if(availableCodes.Count == 0)
                {
                    ModelState.AddModelError("", "Códigos agotados");
                    return BadRequest(ModelState);
                }
                return true;
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
        }

        private async Task<ActionResult<IEnumerable<Code>>> GetCodesRetry(Order order)
        {
            try
            {
                var blockIds = await omsRepository.GetBlockIds(order);
                var codeList = new List<Code>();
                foreach (var blockId in blockIds)
                {
                    var codes = await omsRepository.CodesRetry(order,blockId); 
                    if(codes.Count > 0)
                    {
                       codeList.AddRange(codes);
                    }
                }
                if(codeList.Count == order.Quantity)
                {
                    var codesResult = await _repository.ProcessOrder(codeList);
                    return Ok(codesResult);
                }
                else
                {
                    ModelState.AddModelError("", "Ha ocurrido un error, favor intentar nuevamente");
                    return BadRequest(ModelState);
                }
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
        } 
    }
}
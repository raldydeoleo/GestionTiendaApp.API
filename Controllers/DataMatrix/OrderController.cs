
using System;
using System.Collections.Generic;
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
using System.Net;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace BoxTrackLabel.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrderController : DataMatrixBaseController<Order, OrderRepository>
    {
        private readonly OrderRepository _repository;
        private readonly OmsRepository omsRepository;
        private readonly EmailSender emailSender;
        private readonly EmailAccountRepository emailAccountRepository;
        private readonly IHostingEnvironment hostingEnvironment;

        public OrderController(OrderRepository repository, OmsRepository omsRepository, EmailSender emailSender, EmailAccountRepository emailAccountRepository, IHostingEnvironment hostingEnvironment) : base(repository)
        {
           _repository = repository;
            this.omsRepository = omsRepository;
            this.emailSender = emailSender;
            this.emailAccountRepository = emailAccountRepository;
            this.hostingEnvironment = hostingEnvironment;
        }
        
        [Authorize(Permissions.Menus.DataMatrix_SUPERVISOR)]
        [HttpPost]
        public override async Task<ActionResult<Order>> Post(Order order)
        {
            try
            {
                order.Mrp = order.Mrp==""?"0000":order.Mrp;
                var postResponse = await omsRepository.PostOrder(order);
                var response = postResponse.Item1;
                var responseContent = postResponse.Item2;
                if(response.StatusCode == HttpStatusCode.OK)
                {
                    var postOrderResponse = JsonConvert.DeserializeObject<OrderCreationResponse>(responseContent);
                    order.OrderId = postOrderResponse.orderId;
                    order.ExpectedCompleteTimestamp = postOrderResponse.expectedCompleteTimestamp;
                    var userClaim = HttpContext.User.Claims.Where(c=>c.Type=="http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name").SingleOrDefault();
                    string userName = userClaim != null ? userClaim.Value : null; 
                    order.UserCreate = userName;
                    var postOrderResult = await base.Post(order);
                    var logoPath = Path.Combine(hostingEnvironment.WebRootPath, @"Images/logo117.jpg");
                    //var logoPath = "https://www.laaurora.com.do/wp-content/uploads/2018/06/logo-la-aurora.png";
                    var emailBody = $"Dear Roman,<br><br>"+
                                    $"Could you please confirm the following order?:<br>"+
                                    $"<b>Order Id:</b> {order.OrderId}<br><br>"+
                                    $"<b>Product:</b> {order.ProductDescription}<br><br>"+
                                    $"<b>Quantity:</b> {order.Quantity}<br><br>"+
                                    $"Kind regards,<br><br>";
                                    // $"<a href='https://www.laaurora.com.do'><img src='{logoPath}' style='width:192px;height:96px'></img></a>";
                    await emailSender.SendBulkEmailAsync(await emailAccountRepository.GetAll(),"BoxTrackLabel - Marking Codes Order Created",emailBody);
                    return  postOrderResult;
                }
                else
                {
                    var translateRequestObject = new {sourceLang = "ru", targetLang="es", sourceText = responseContent}; 
                    var translatedStringMessage = await omsRepository.TranslateThisAsync(translateRequestObject);
                    Log.Error("Error durante la creación de la orden: {0}",translatedStringMessage);
                    var errorMessage = FormatErrorMessage(translatedStringMessage);
                    ModelState.AddModelError("", errorMessage);
                    return BadRequest(ModelState);
                    //return StatusCode((int)response.StatusCode,response.ReasonPhrase);
                } 
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
        }
        [Route("GetByDate")] 
        [Authorize(Permissions.Menus.DataMatrix_OPERADOR)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetByDate(DateTime date)
        {
            try
            {
                return await _repository.GetByDate(date);
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
            
        }
        [Route("CheckAvailability")] 
        [HttpPost]
        [Authorize(Permissions.Menus.DataMatrix_SUPERVISOR)]
        public async Task<ActionResult<IEnumerable<Order>>> CheckAvailability(Order order)
        {
            try
            {
                var availabilityResponseObject = await omsRepository.CheckAvailability(order);
                var response = availabilityResponseObject.Item1;
                var responseContent = availabilityResponseObject.Item2;
                if(response.StatusCode == HttpStatusCode.OK)
                {
                    var availabilityResponse = JsonConvert.DeserializeObject<OrderAvailabilityResponse>(responseContent);
                    var orderInfo = availabilityResponse.orderInfos.Where(o=>o.orderId == order.OrderId).SingleOrDefault();
                    if(orderInfo != null)
                    {
                        if(orderInfo.orderStatus == "READY")
                        {
                            await _repository.UpdateOrderStatus(order.Id, "Disponible");
                            var orders = await _repository.GetByDate(order.CreationDate.Date);
                            return Ok(orders);
                        }
                        else if(orderInfo.orderStatus == "EXPIRED")
                        {
                            await _repository.UpdateOrderStatus(order.Id, "Disponible");
                            var orders = await _repository.GetByDate(order.CreationDate.Date);
                            ModelState.AddModelError("", "La orden ha expirado");
                            return  BadRequest(ModelState);
                        }
                        else if(orderInfo.orderStatus == "DECLINED")
                        {
                            var translateObject = new {sourceLang = "ru", targetLang="es", sourceText = orderInfo.declineReason}; 
                            var translatedReason = await omsRepository.TranslateThisAsync(translateObject);
                            await _repository.UpdateOrderStatus(order.Id,"Rechazada","La orden fue rechazada, motivo: "+translatedReason);
                            ModelState.AddModelError("", "La orden fue rechazada por el OMS, Motivo:</br>"+translatedReason);
                            return  BadRequest(ModelState);
                        }
                        else
                        {
                            ModelState.AddModelError("", "Los códigos aún no están disponibles, favor intentar más tarde");
                            return  BadRequest(ModelState);
                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    var translateRequestObject = new {sourceLang = "ru", targetLang="es", sourceText = responseContent}; 
                    var translatedStringMessage = await omsRepository.TranslateThisAsync(translateRequestObject);
                    Log.Error("Error verificando disponibilidad de orden, detalles: {0}",translatedStringMessage);
                    var errorMessage = FormatErrorMessage(translatedStringMessage);
                    ModelState.AddModelError("", errorMessage);
                    return BadRequest(ModelState);
                }
            }
            catch(DbUpdateConcurrencyException dbcex)
            {
                Log.Error(dbcex,dbcex.Message);
                ModelState.AddModelError("", "La orden ya fue actualizada por otro usuario o proceso en segundo plano");
                return BadRequest(ModelState);
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
        }
        [Route("AuthorizePrint")] 
        [HttpPut]
        [Authorize(Permissions.Menus.DataMatrix_SUPERVISOR)]
        public async Task<ActionResult<Order>> AuthorizePrint(Order order) 
        {
            try
            {
                var userClaim = HttpContext.User.Claims.Where(c=>c.Type==Values.UserClaimKey).SingleOrDefault();
                string userName = userClaim != null ? userClaim.Value : null;
                var orderUpdateResult = await _repository.AuthorizeOrder(order.Id, userName);
                return Ok(order);
            }
            catch(DbUpdateConcurrencyException dbcex)
            {
                Log.Error(dbcex,dbcex.Message);
                ModelState.AddModelError("", "El registro de orden fue modificado por otro usuario o proceso de manera simultánea , favor intentar nuevamente");
                return BadRequest(ModelState);
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
        }  

        [Route("CloseOrder")] 
        [HttpPut]
        [Authorize(Permissions.Menus.DataMatrix_SUPERVISOR)]
        public async Task<ActionResult<Order>> CloseOrder(Order order)
        {
            try
            {
                var postUtilisationResponse = await omsRepository.SendUtilisationReport(order);
                var utilisationResponse = postUtilisationResponse.Item1;
                var utilisationResponseContent = postUtilisationResponse.Item2;
                if(utilisationResponse.StatusCode == HttpStatusCode.OK)
                {
                    //var postDropOutResponse = await SendDropoutReport(order);
                    //var dropOutResponse = postDropOutResponse.Item1;
                    //var dropOutResponseContent = postDropOutResponse.Item2;
                    //if(dropOutResponse.StatusCode == HttpStatusCode.OK)
                    //{
                        var closeOrderResponse = await omsRepository.CloseOrderBuffer(order);
                        var closeResponse = closeOrderResponse.Item1;
                        var closeResponseContent = closeOrderResponse.Item2;
                        if(closeResponse.StatusCode == HttpStatusCode.OK)
                        {
                            return Ok(await CloseLocalOrder(order));
                        }
                        else
                        {
                            var translateRequestObject = new {sourceLang = "ru", targetLang="es", sourceText = closeResponseContent}; 
                            var translatedStringMessage = await omsRepository.TranslateThisAsync(translateRequestObject);
                            Log.Error("Error durante el cierre de orden, detalles: {0}",translatedStringMessage);
                            var errorMessage = FormatErrorMessage(translatedStringMessage);
                            ModelState.AddModelError("", errorMessage);
                            return BadRequest(ModelState);
                        }
                    //}
                    //else
                    //{
                        //Log.Error("Error durante el reporte de desechados MC, detalles: {0}",dropOutResponseContent);
                       // ModelState.AddModelError("", dropOutResponseContent);
                     //   return BadRequest(dropOutResponseContent);
                   // }
                }
                else
                {
                    
                    var translateRequestObject = new {sourceLang = "ru", targetLang="es", sourceText = utilisationResponseContent}; 
                    var translatedStringMessage = await omsRepository.TranslateThisAsync(translateRequestObject);
                    Log.Error("Error durante el reporte de utilización MC, detalles: {0}",translatedStringMessage);
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
        private async Task<Order> CloseLocalOrder(Order order)
        {
            var userClaim = HttpContext.User.Claims.Where(c=>c.Type=="http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name").SingleOrDefault();
            string userName = userClaim != null ? userClaim.Value : null; 
            return await _repository.CloseOrder(order.Id,userName);
        }
        // private string TranslateApiError(string responseContent)
        // {
        //     var response = "";
        //     var def =  new {
        //         success = "",
        //         fieldErrors = new []{new{fieldError="",fieldName="",errorCode=0}},
        //         globalErrors = new [] {new{error="",errorCode=0}}
        //     }; 
        //     var errorResponse = JsonConvert.DeserializeAnonymousType(responseContent,def);
        //     var errorsFound = errorResponse.fieldErrors.Where(e => e.errorCode  == 7140 || e.errorCode == 6010).ToList();
        //     if(errorsFound.Count > 0)
        //     {
        //         response = "No se enviaron códigos confirmados durante el envío del reporte de utilización";
        //     }
        //     else
        //     {
        //         response = "Ha ocurrido un error realizando el reporte de códigos utilizados";
        //     }
        //     return response;
        // }
       
    }
}
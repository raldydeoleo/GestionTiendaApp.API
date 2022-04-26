using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoxTrackLabel.API.Models;
using BoxTrackLabel.API.Models.BoxTrack;
using BoxTrackLabel.API.Repositories;
using BoxTrackLabel.API.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace BoxTrackLabel.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductionController : ControllerBase
    {
        private readonly ProductionRepository _productionRepository;
        private readonly ShiftRepository _shiftRepository;
        private readonly OrderRepository _orderRepository;

        //private readonly CustomerRepository customerRepository;

        public ProductionController(ProductionRepository productionRepository, ShiftRepository shiftRepository, OrderRepository orderRepository, CodesRepository codesRepository)        
        {
            _productionRepository = productionRepository;
            _shiftRepository = shiftRepository;
            _orderRepository = orderRepository;
            // this.customerRepository = customerRepository;
        }

        [HttpPost]
        [Route("openshift")]
        [Authorize(Permissions.Menus.Etiquetado)]
        public async Task<ActionResult<Production>> OpenShift(OpenShiftRequest req)
        {
            try
            {
                var productionTuple = await InitProduction(req);
                var production =  productionTuple.Item1;
                var shift = productionTuple.Item2;
                var shifts = productionTuple.Item3;
                var currentDate = productionTuple.Item4;
                var scheduleProduct = await _productionRepository.GetScheduleProduct(production);
                if(scheduleProduct == null)
                {
                    ModelState.AddModelError("", "El módulo seleccionado no contiene producto programado");
                    return BadRequest(ModelState);
                }
                var productionExist = await _productionRepository.GetProduction(production.IdProceso, production.IdModulo, production.IdTurno, production.FechaProduccion,true);
                if (productionExist != null){
                    if(productionExist.IdProducto != null)
                    {
                        if(scheduleProduct.IdProducto != productionExist.IdProducto)
                        {
                            var result = await _productionRepository.ChangeOverProduction(productionExist, scheduleProduct);
                            return Ok(result);
                        }  
                    }
                    else
                    {
                        productionExist.IdProducto = scheduleProduct.IdProducto;
                    }
                    return Ok(productionExist);
                }
                else{
                    productionExist = await _productionRepository.GetProductionByChangeOver(production.IdProceso, production.IdModulo, production.IdTurno, production.FechaProduccion,true);
                    if(productionExist != null)
                    {
                        if(shift.Id != shifts.Last().Id)
                        {
                            var index = shifts.FindIndex(s => s.Id == shift.Id);
                            production.IdTurno = shifts.ElementAt(index + 1).Id;
                        }
                        else
                        {
                            production.IdTurno = shifts.First().Id;
                            production.FechaProduccion = currentDate;
                        }
                    }
                    production.IdProducto = scheduleProduct.IdProducto;
                    await _productionRepository.Add(production);
                }
                return Created("api/production/openshift",production);
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            } 
        }

        [HttpPut]
        [Route("closeshift")]
        [Authorize(Permissions.Menus.Etiquetado)]
        public async Task<ActionResult<Production>> CloseShift(Production production)
        {
            try
            {
                await _productionRepository.CloseProduction(production);
                return StatusCode(200);
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
        }

        
        [HttpGet]
        [Route("getproduction/{productionId}")]
        [Authorize(Permissions.Menus.Etiquetado)]
        public async Task<ActionResult<Production>> GetProduction(int productionId)
        {
            try
            {
                return await _productionRepository.GetProductionById(productionId);
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
        }

        // [HttpGet]
        // [Route("getcustomers")]
        // [Authorize(Permissions.Menus.Etiquetado)]
        // public async Task<ActionResult<List<Customer>>> GetCustomers()
        // {
        //     return await customerRepository.GetCustomers();
        // }

        // [HttpGet]
        // [Route("getcustomersbyterm/{term}")]
        // [Authorize(Permissions.Menus.Etiquetado)]
        // [AllowAnonymous]
        // public async Task<ActionResult<List<Customer>>> GetCustomersByTerm(string term)
        // {
        //     return await customerRepository.GetCustomersByTerm(term);
        // }

        [HttpPost]
        [Route("getshiftdatamatrix")]
        [Authorize(Permissions.Menus.Etiquetado)]
        public async Task<ActionResult<Production>> GetShiftDataMatrix(OpenShiftRequest req)
        {
            try
            {
                var productionTuple = await InitProduction(req);
                var production = productionTuple.Item1;
                var shift = productionTuple.Item2;
                var shifts = productionTuple.Item3;
                var currentDate = productionTuple.Item4;
                var dataMatrixOrder = await _orderRepository.Get(req.DataMatrixOrderId);
                production.DataMatrixOrderId = req.DataMatrixOrderId;
                production.IdProducto =  dataMatrixOrder.ProductCode;
                var productionExist = await _productionRepository.GetProductionByOrder(production.IdProceso, production.IdModulo, production.IdTurno, production.FechaProduccion,true, req.DataMatrixOrderId);
                if(productionExist != null)
                {
                    return Ok(productionExist);
                }
                else
                {
                    await _productionRepository.Add(production);
                    return Created("api/production/getshiftdatamatrix",production);
                }
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
            
        }

        [HttpGet]
        [Route("checkProductionByOrder/{orderId}")]
        [Authorize(Permissions.Menus.Etiquetado)]
         public async Task<ActionResult<Production>> CheckProductionByOrder(int orderId)
         {
             try
             {
                 var production = await _productionRepository.GetProductionByOrderId(orderId);
                 return Ok(production);
             }
             catch (System.Exception ex)
             {
                 return ManageException(ex);
             }
             
         }
 
        private async Task<Tuple<Production,Shift,List<Shift>,DateTime>> InitProduction(OpenShiftRequest req)
        {
            var production = new Production();
            var currentDate = DateTime.Now;
            var currentTime = currentDate.TimeOfDay;
            production.TurnoAbierto = true;
            production.FechaHoraAperturaTurno = currentDate;
            var shift = await _shiftRepository.GetCurrentShift(currentTime); 
            var productionDate = currentDate;
            var shifts = await _shiftRepository.GetShifts();
            if(shift.Id == shifts.Last().Id)
            {
                var midNightTime = new TimeSpan(00,00,00);
                var lastShiftEndTime = new TimeSpan(06,00,00);
                if(currentTime >= midNightTime && currentTime < lastShiftEndTime)
                {
                    productionDate = currentDate.AddDays(-1);
                }
            }
            production.IdModulo = req.IdModulo;
            production.IdProceso = req.IdProceso;
            production.UsuarioAperturaTurno = req.UsuarioApertura;
            production.IdTurno = shift.Id;
            production.FechaProduccion = productionDate;
            return Tuple.Create(production,shift,shifts,currentDate);
        }
        private ActionResult ManageException(Exception ex)
        {
            Log.Error(ex,ex.Message);
            ModelState.AddModelError("", "Error interno. Favor de intentarlo nuevamente.");
            return BadRequest(ModelState);
        }

    }
}
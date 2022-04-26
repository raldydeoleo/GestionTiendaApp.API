using BoxTrackLabel.API.Models;
using BoxTrackLabel.API.Repositories;
using BoxTrackLabel.API.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Serilog;

namespace BoxTrackLabel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ScheduleController: ControllerBase
    {
        private readonly ScheduleRepository scheduleRepository;
        private readonly ProductsRepository productsRepository;
        private readonly ProcessRepository processRepository;
        private readonly ModuleRepository moduleRepository;
        private readonly OmsRepository omsRepository;

        public ScheduleController(ScheduleRepository scheduleRepository, ProductsRepository productsRepository, ProcessRepository processRepository, ModuleRepository moduleRepository, OmsRepository omsRepository)
        {
            this.scheduleRepository = scheduleRepository;
            this.productsRepository = productsRepository;
            this.processRepository = processRepository;
            this.moduleRepository = moduleRepository;
            this.omsRepository = omsRepository;
        }
        [HttpGet]
        [Route("getschedulesbydate")] 
        [Authorize(Permissions.Menus.Programacion)]
        public async Task<ActionResult<List<Schedule>>> GetSchedulesByDate(DateTime productionDate, string estatus, string idproceso, string idmodulo)
        {
            try
            {
                return await scheduleRepository.GetSchedulesByDate(productionDate, estatus, idproceso, idmodulo);
            }
            catch(SqlException ex)
            {
                Log.Error(ex,ex.Message);
                var translateRequestObject = new {sourceLang = "en", targetLang="es", sourceText = ex.Message}; 
                var translatedStringMessage = await omsRepository.TranslateThisAsync(translateRequestObject);
                ModelState.AddModelError("","Error interno: "+translatedStringMessage+" Contacte a tecnología");
                return BadRequest(ModelState);
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
            
        }
        [HttpPost]
        [Authorize(Permissions.Menus.Programacion)]
        public async Task<ActionResult<Schedule>> CreateSchedule(Schedule schedule)
        {
            try
            {
                Schedule result = null;
                if(!await isGoodProduct(schedule.IdProducto))
                {
                    ModelState.AddModelError("", "Código de producto incorrecto");
                    return BadRequest(ModelState);
                }
                var scheduleExistsSameProduct = await scheduleRepository.SameProductScheduleExists(schedule.FechaProduccion, schedule.IdProceso, schedule.IdModulo, schedule.IdTurno, schedule.IdProducto);
                if (scheduleExistsSameProduct)
                {
                    ModelState.AddModelError("", "Este es el producto actualmente programado");
                    return BadRequest(ModelState);
                }
                var scheduleExists = await scheduleRepository.ScheduleExists(schedule.FechaProduccion, schedule.IdProceso, schedule.IdModulo, schedule.IdTurno, schedule.IdProducto);
                if (scheduleExists)
                {
                    return StatusCode(409); //conflicto
                }
                
                result = await scheduleRepository.CreateSchedule(schedule);  
                
                //catch (DbUpdateException ex)
                //{
                //    SqlException innerException = ex.InnerException as SqlException;
                //    if (innerException != null && innerException.Number == 2627)
                //    {
                //        ModelState.AddModelError("", "Esta programación ya ha sido registrada");
                //        return BadRequest(ModelState);
                //    }
                //    else
                //    {
                //        throw;
                //    }
                //}
                return Created("api/schedule",result);
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }            
        }


        [HttpPut]
        [Authorize(Permissions.Menus.Programacion)]
        public async Task<ActionResult<Schedule>> UpdateSchedule(Schedule schedule)
        {
            try
            {
                if (!await CanEditSchedule(schedule))
                {
                    //ModelState.AddModelError("", "Ya se generaron etiquetas, puede finalizar y crear una programación nueva");
                    //return BadRequest(ModelState);
                    return StatusCode(409); //conflicto
                }
                if (!await isGoodProduct(schedule.IdProducto))
                {
                    ModelState.AddModelError("", "Código de producto incorrecto");
                    return BadRequest(ModelState);
                }
                var shedule = await scheduleRepository.UpdateSchedule(schedule);
                return Ok(shedule);
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
            
        }
        [HttpPost]
        [Route("insertschedules")]
        [Authorize(Permissions.Menus.Programacion)]
        public async Task<ActionResult> InsertSchedules(List<Schedule> schedules)
        {
            try
            {
                var conflictSchedules = await GetSchedulesWithConflict(schedules);
                var schedulesToSave = schedules.Except(conflictSchedules).ToList();
                if (schedulesToSave.Count > 0)
                {
                    try
                    {
                        await scheduleRepository.InsertScheduleList(schedulesToSave);
                    }
                    catch (SqlException ex)
                    {
                        //else if (ex.Number == 2627)
                        //{
                        //    ModelState.AddModelError("", "Existen Programaciones Duplicadas");
                        //    return BadRequest(ModelState);
                        //}
                        if (ex.Number == 19 || ex.Number == 26)
                        {
                            ModelState.AddModelError("", "Error de conexión con la base de datos");
                            return BadRequest(ModelState);
                        }
                        else
                        {
                            ModelState.AddModelError("", "Ha ocurrido un error al procesar su solicitud, favor intentar nuevamente");
                            return BadRequest(ModelState);
                        }
                    }
                    

                }

                return Ok(conflictSchedules);
            }
            catch(Exception ex)
            {
                return ManageException(ex);
            }
        }
        [HttpPost]
        [Route("changeschedule")]
        [Authorize(Permissions.Menus.Programacion)]
        public async Task<ActionResult> ChangeSchedule(Schedule schedule)
        {
            try
            {
                var changeSuccesfully = await scheduleRepository.TryChangeSchedule(schedule);
                if (changeSuccesfully)
                {
                    return Ok();
                }
                else
                {
                    ModelState.AddModelError("", "Ha ocurrido un error inesperado, favor intentar nuevamente");
                    return BadRequest(ModelState);
                }
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
            
        }
        [HttpPut]
        [Route("finishschedule")]
        [Authorize(Permissions.Menus.Programacion)]
        public async Task<ActionResult<Schedule>> FinishSchedule(Schedule schedule)
        {
            try
            {
                schedule.Finalizado = true;
                schedule.FechaHoraFinalizado = DateTime.Now;
                var shedule = await scheduleRepository.UpdateSchedule(schedule);
                return Ok(shedule);
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
            
        }
        [Route("getprocesses")]
        [Authorize(Permissions.Menus.Programacion)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Process>>> GetProcesses()
        {
            try
            {
                var processes = await processRepository.GetAll();
                return processes;
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
            
        }

        [Route("getmodules")]
        [Authorize(Permissions.Menus.Programacion)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Module>>> GetModules()
        {
            try
            {
                var modules = await moduleRepository.GetAll();
                return modules;
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
        }
        [Route("getproductsbyname/{name}")]
        [Authorize(Permissions.Menus.Programacion)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByName(string name)
        {
            try
            {
                var products = await productsRepository.GetByName(name);
                return products;
            }
            catch(SqlException ex)
            {
                Log.Error(ex,ex.Message);
                var translateRequestObject = new {sourceLang = "en", targetLang="es", sourceText = ex.Message}; 
                var translatedStringMessage = await omsRepository.TranslateThisAsync(translateRequestObject);
                ModelState.AddModelError("","Error interno: "+translatedStringMessage+" Contacte a tecnología");
                return BadRequest(ModelState);
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
            
        }
        [Route("getproducts")]
        [Authorize(Permissions.Menus.Programacion)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            try
            {
                var products = await productsRepository.GetAll();
                return products;
            }
            catch(SqlException ex)
            {
                Log.Error(ex,ex.Message);
                var translateRequestObject = new {sourceLang = "en", targetLang="es", sourceText = ex.Message}; 
                var translatedStringMessage = await omsRepository.TranslateThisAsync(translateRequestObject);
                ModelState.AddModelError("","Error interno: "+translatedStringMessage+" Contacte a tecnología");
                return BadRequest(ModelState);
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
        }
        private async Task<bool> CanEditSchedule(Schedule schedule)
        {
            var canEdit = await scheduleRepository.CanEditSchedule(schedule);
            return canEdit;
        }

        private async Task<bool> isGoodProduct(string idProducto)
        {
            var product = await productsRepository.Get(idProducto);
            if (product == null)
            {
                return false;
            }
            return true;
        }
        private async Task<List<Schedule>> GetSchedulesWithConflict(List<Schedule> schedules)
        {
            List<Schedule> schedulesWithConflict = new List<Schedule>();
            foreach (var schedule in schedules)
            {
                var exists = await scheduleRepository.ScheduleExistsOnShift(schedule.FechaProduccion, schedule.IdProceso, schedule.IdModulo, schedule.IdTurno);
                if (exists)
                {
                    schedulesWithConflict.Add(schedule);
                }
            }
            return schedulesWithConflict;
        }

        private ActionResult ManageException(Exception ex)
        {
            Log.Error(ex,ex.Message);
            ModelState.AddModelError("", "Error interno. Favor de intentarlo nuevamente.");
            return BadRequest(ModelState);
        }

    }
}


using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BoxTrackLabel.API.Models;
using BoxTrackLabel.API.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace BoxTrackLabel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public abstract class MyBaseController<TEntity, TRepository> : ControllerBase
        where TEntity : class, IEntity
        where TRepository : IRepository<TEntity>
    {
        private readonly TRepository repository;
        public MyBaseController(TRepository repository)
        {
            this.repository = repository;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TEntity>>> Get()
        {
            try
            {
                return await repository.GetAll();
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<TEntity>> Get(int id)
        {
            try
            {
                var entity = await repository.Get(id);
                if (entity == null)
                {
                    return NotFound();
                }
                return entity;
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
            
        } 


        [HttpPut]
        public async Task<IActionResult> Put(TEntity entity)
        {
            if (await repository.IsCodeUnavailable(entity))
            {
                ModelState.AddModelError("", "El código ingresado ya está en uso");
                return BadRequest(ModelState);
            }
            entity.FechaHoraModificacion = DateTime.Now;
            try
            {
                await repository.Update(entity);
            }
            catch (Exception ex)
            {

                return ManageException(ex);
            }
            return NoContent();
        }
        [HttpPost]
        public async Task<ActionResult<TEntity>> Post(TEntity entity)
        { 
            entity.FechaHoraModificacion = DateTime.Now;
            try
            {
                if (await repository.IsCodeUnavailable(entity))
                {
                    ModelState.AddModelError("", "El código ingresado ya está en uso");
                    return BadRequest(ModelState);
                }
                await repository.Add(entity);
            }
            catch (DbUpdateException ex)
            {
                SqlException innerException = ex.InnerException as SqlException;
                if (innerException != null && innerException.Number == 2627)
                {
                    ModelState.AddModelError("", "El código ingresado ya está en uso");
                    return BadRequest(ModelState);
                }
                else
                {
                    return ManageException(ex);
                }
            }
            catch (Exception e)
            {
                return ManageException(e);
            }
            
            return StatusCode(201);
        }
        [HttpPut]
        [Route("delete")]
        public async Task<ActionResult<TEntity>> Delete(TEntity entity)
        {
            try
            {
                entity.EstaBorrado = true;
                entity.FechaHoraBorrado = DateTime.Now;
                await repository.Update(entity);
                return entity;
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
           
        }
        protected ActionResult ManageException(Exception ex)
        {
            Log.Error(ex,ex.Message);
            ModelState.AddModelError("", "Error interno. Favor de intentarlo nuevamente.");
            return BadRequest(ModelState);
        }
    }
}
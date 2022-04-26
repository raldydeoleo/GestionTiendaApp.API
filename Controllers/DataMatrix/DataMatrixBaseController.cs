
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BoxTrackLabel.API.Repositories;
using BoxTrackLabel.API.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;

namespace BoxTrackLabel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public abstract class DataMatrixBaseController<TEntity, TRepository> : ControllerBase
        where TEntity : class, IDataMatrixEntity
        where TRepository : IDataMatrixRepository<TEntity>
    {
        private readonly TRepository repository;
        public DataMatrixBaseController(TRepository repository)
        {
            this.repository = repository;
        }
        /// <summary>
        /// Método para consultar todos los regitros de una entidad determinada (clic para ver detalles)
        /// </summary>
        /// <remarks>
        /// Ejemplo de petición:
        ///
        ///     GET /Code    :se obtienen todos los códigos descargados
        ///     GET /Order   :se obtienen todos las ordernes descargados
        ///     GET /EmailAccount  :se obtienen todas las cuentas de correo registradas
        ///     GET /OrderSettings  : se obtienen todos los valores por defecto para las órdenes
        /// </remarks>
        /// <response code="200">Se obtiene un listado de elementos acompañados del código de estado 200, los elementos tendrán la siguiente estructura:</response>
        /// <response code="400">Se ha lanzado un excepción</response>
        /// <response code="401">Acceso no autorizado: el token correcto no fue proporcionado o está vencido</response>
        /// <response code="403">Acceso prohibido: el usuario no tiene privilegios para acceder al presente método</response>
        [HttpGet]
        [Authorize(Permissions.Menus.DataMatrix_OPERADOR)]
        public virtual async Task<ActionResult<IEnumerable<TEntity>>> Get()
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
        /// <summary>
        /// Método para consultar un registro de una entidad determinada mediante su clave (clic para ver detalles)
        /// </summary>
        /// <remarks>
        /// Ejemplo de petición:
        ///
        ///     GET /Code/{id}    :se obtiene un registro de código mediante su id
        ///     GET /Order/{id}   :se obtiene un registro de orden mediante su id
        ///     GET /EmailAccount/{id}  :se obtiene correo registrado mediante su id
        ///     GET /OrderSettings/{id}  : se obtiene un registro de los valores por defecto de las órdenes
        /// </remarks>
        /// <response code="200">Se obtiene un registro específico junto al código de estado 200, en elemento tendrá la siguiente estructura:</response>
        /// <response code="400">Se ha lanzado un excepción</response>
        /// <response code="401">Acceso no autorizado: el token correcto no fue proporcionado o está vencido</response>
        /// <response code="403">Acceso prohibido: el usuario no tiene privilegios para acceder al presente método</response>
        [HttpGet("{id}")]
        [Authorize(Permissions.Menus.DataMatrix_OPERADOR)]
        public virtual async Task<ActionResult<TEntity>> Get(int id)
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
        /// <summary>
        /// Método para actualizar un registro de una entidad determinada (clic para ver detalles)
        /// </summary>
        /// <remarks>
        /// Ejemplo de petición:
        ///
        ///     Importante: El parámetro del método es un objeto que almacena la entidad a modificar. 
        ///
        ///     PUT /Code    :actualiza un registro de códigos datamatrix 
        ///     PUT /Order   :actualiza un registro de órdenes datamatrix
        ///     PUT /EmailAccount  :actualiza un registro de cuentas de correo
        ///     PUT /OrderSettings  : actualiza un registro de los valores por defecto de las órdenes
        /// </remarks>
        /// <response code="200">Se obtiene el registro actualizado junto al código de estado 200, en elemento tendrá la siguiente estructura:</response>
        /// <response code="400">Se ha lanzado un excepción</response> 
        /// <response code="401">Acceso no autorizado: el token correcto no fue proporcionado o está vencido</response>
        /// <response code="403">Acceso prohibido: el usuario no tiene privilegios para acceder al presente método</response>
        [HttpPut]
        [Authorize(Permissions.Menus.DataMatrix_SUPERVISOR)]
        public virtual async Task<IActionResult> Put(TEntity entity)
        {
            try
            {
                await repository.Update(entity);
                return NoContent();
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
            
        }
        /// <summary>
        /// Método para crear un registro de una entidad determinada (clic para ver detalles)
        /// </summary>
        /// <remarks>
        /// Ejemplo de petición:
        ///
        ///     Importante: El parámetro del método es un objeto que almacena la entidad a crear. 
        ///
        ///     POST /Code    :Crea un registro de códigos datamatrix 
        ///     POST /Order   :Crea un registro de órdenes datamatrix
        ///     POST /EmailAccount  :Crea un registro de cuentas de correo
        ///     POST /OrderSettings  : Crea un registro de los valores por defecto de las órdenes
        /// </remarks>
        /// <response code="200">Se obtiene el registro creado junto al código de estado 200, en elemento tendrá la siguiente estructura:</response>
        /// <response code="400">Se ha lanzado un excepción</response> 
        /// <response code="401">Acceso no autorizado: el token correcto no fue proporcionado o está vencido</response>
        /// <response code="403">Acceso prohibido: el usuario no tiene privilegios para acceder al presente método</response>
        [HttpPost]
        [Authorize(Permissions.Menus.DataMatrix_SUPERVISOR)]
        public virtual async Task<ActionResult<TEntity>> Post(TEntity entity)
        { 
        //     try
        //     {
            await repository.Add(entity);
            return Created("api/Order",entity);
            // }
            // catch (System.Exception ex)
            // {
            //     return ManageException(ex);
            // }
            
        }
         /// <summary>
        /// Método para eliminar un registro de una entidad determinada mediante su clave (clic para ver detalles)
        /// </summary>
        /// <remarks>
        /// Ejemplo de petición:
        ///
        ///     DELETE /Code/{id}    :elimina un registro de código mediante su id
        ///     DELETE /Order/{id}   :elimina un registro de orden mediante su id
        ///     DELETE /EmailAccount/{id}  :elimina correo registrado mediante su id
        ///     DELETE /OrderSettings/{id}  : elimina un registro de los valores por defecto de las órdenes
        /// </remarks>
        /// <response code="200">Se elimina un registro específico junto al código de estado 200, en elemento tendrá la siguiente estructura:</response>
        /// <response code="400">Se ha lanzado un excepción</response>
        /// <response code="401">Acceso no autorizado: el token correcto no fue proporcionado o está vencido</response>
        /// <response code="403">Acceso prohibido: el usuario no tiene privilegios para acceder al presente método</response>
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Permissions.Menus.DataMatrix_SUPERVISOR)]
        public virtual async Task<ActionResult<TEntity>> Delete(int id)
        {
            try
            {
                var entity = await repository.Delete(id);
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

        protected string FormatErrorMessage(string translatedStringMessage)
        {
            var def = new {
                fieldErrors = new []{new{fieldError="",fieldName="",errorCode=0}},
                globalErrors = new [] {new{error="",errorCode=0}}
            };
            var translatedTextObject = JsonConvert.DeserializeAnonymousType(translatedStringMessage,def);
            var errorMessage = "";
            if(translatedTextObject.fieldErrors != null)
            {
                foreach (var error in translatedTextObject.fieldErrors)
                {
                    errorMessage += "* "+error.fieldError + ".</br>";
                }
            }
            if(translatedTextObject.globalErrors != null)
            {
                foreach (var error in translatedTextObject.globalErrors)
                {
                    errorMessage += "* "+error.error + ".</br>";
                }
            }
            if(errorMessage == "")
            {
                errorMessage = translatedStringMessage;
            }
            return errorMessage;
        }

    }
}
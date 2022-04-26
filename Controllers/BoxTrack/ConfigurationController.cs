using System;
using System.IO;
using System.Threading.Tasks;
using BoxTrackLabel.API.Models;
using BoxTrackLabel.API.Repositories;
using BoxTrackLabel.API.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace BoxTrackLabel.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ConfigurationController : MyBaseController<ConfigurationValue, ConfigurationRepository>
    {
        private readonly ConfigurationRepository _repository;
        private readonly IHostingEnvironment  _hostEnvironment;

        public ConfigurationController(ConfigurationRepository repository,IHostingEnvironment  hostEnvironment) : base(repository)
        {
           _repository = repository;
           _hostEnvironment = hostEnvironment;
        }
        /// <summary>
        /// Método para actualizar un registro de configuración de la aplicación
        /// </summary>
        /// <remarks>
        /// Ejemplo de petición:
        ///     
        ///     Importante: se envía por parámetros el objeto que representa el registro de configuración a modificar
        ///
        ///     PUT /updateconfig    
        ///     {
        ///          "Id": 1,
        ///          "Codigo": "confirmarModulo",
        ///          "TextoConfiguracion": "Confirmación de módulo en impresión de etiquetas",
        ///          "ValorConfiguracion": "False",
        ///          "EstaBorrado": false,
        ///          "FechaHoraBorrado": null,
        ///          "FechaHoraModificacion": null,
        ///          "FechaHoraRegistro": null,
        ///          "UsuarioRegistro": "Juan",
        ///          "UsuarioModificacion": null,
        ///          "UsuarioEliminacion": null
        ///     }
        /// </remarks>
        /// <param value="configuration">Registro de valor de configuración del sistema</param>
        /// <response code="200">Código de estado 200, que indica que se ha actualizado la configuración exitosamente</response>
        /// <response code="400">Si se intenta duplicar un código de configuración o si se ha lanzado un excepción</response> 
        /// <response code="401">Acceso no autorizado: el token correcto no fue proporcionado o está vencido</response>
        /// <response code="403">Acceso prohibido: el usuario no tiene privilegios para acceder al presente método</response>
        [HttpPut]
        [Route("updateconfig")]
        [Authorize(Permissions.Menus.ConfigurarEquipo)]
        public async Task<IActionResult> UpdateConfig(ConfigurationValue configuration)
        {
            if (await _repository.IsCodeUnavailable(configuration))
            {
                ModelState.AddModelError("", "El código ingresado ya está en uso");
                return BadRequest(ModelState);
            }
            configuration.FechaHoraModificacion = DateTime.Now;
            try
            {
                await _repository.Update(configuration);
            }
            catch (Exception ex)
            {
                return ManageException(ex);
            }
            return NoContent();
        }
        /// <summary>
        /// Método para consultar un registro de configuración de la aplicación mediante su código
        /// </summary>
        /// <remarks>
        /// Ejemplo de petición:
        ///     
        ///     GET /getbycode/confirmarModulo  
        /// </remarks>
        /// <param value="code">Código de un registro de configuración del sistema</param>
        /// <response code="200">Código de estado 200 junto al registro de configuración consultado</response>
        /// <response code="400">si se ha lanzado un excepción</response> 
        /// <response code="401">Acceso no autorizado: el token correcto no fue proporcionado o está vencido</response>
        /// <response code="403">Acceso prohibido: el usuario no tiene privilegios para acceder al presente método</response>
        [HttpGet]
        [Route("getbycode/{code}")]
        [Authorize(Permissions.Menus.ConfigurarEquipo)]
        public async Task<ActionResult<ConfigurationValue>> GetByCode(string code)
        {
            try
            {
                var entity = await _repository.GetByCode(code);
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
        /// Ejecuta el job que actualiza la base de datos maestros de Sql desde SAP 
        /// </summary>
        /// <remarks>
        /// Ejemplo de petición:
        ///     
        ///     GET /updateMasterData  
        /// </remarks>
        /// <response code="200">El job de actualización se ejecutó exitosamente</response>
        /// <response code="400">si se ha lanzado un excepción</response> 
        /// <response code="401">Acceso no autorizado: el token correcto no fue proporcionado o está vencido</response>
        /// <response code="403">Acceso prohibido: el usuario no tiene privilegios para acceder al presente método</response>
        [HttpGet]
        [Route("updateMasterData")]
        [Authorize(Permissions.Menus.ActualizarDatosMaestros)]
        public ActionResult UpdateMasterData()
        {
            try
            {
                //SqlJobManager.InitJob();
                SqlJobManager.Execute();
                return Ok();
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
            
        }

        [HttpGet]
        [Route("DownLoadLocalService")]
        [Authorize(Permissions.Menus.ConfigurarEquipo)]
        public async Task<ActionResult> DownLoadLocalService()
        {
            var wwwrootPath = _hostEnvironment.WebRootPath;
            var path = Path.Combine(wwwrootPath, @"Servicio_Impresión\Servicio_impresión_local.zip");
            var zipFile = await GenerateFile(path, "application/zip");
            zipFile.FileDownloadName = "Servicio_local_impresión.zip";
            return zipFile;
        }
        [HttpPost]
         [Route("DownLoadManual")]
        [Authorize(Permissions.Menus.Etiquetado)]
        public async Task<ActionResult> DownLoadManual([FromBody]JObject fileObjectName)
        {
            var wwwrootPath = _hostEnvironment.WebRootPath;
            var path = Path.Combine(wwwrootPath, @"Manual\"+fileObjectName["fileName"]); 
            var pdfFile =  await GenerateFile(path, "application/pdf");
            pdfFile.FileDownloadName = "Manual_de_usuario.pdf";
            return pdfFile;
        } 
        
        private async Task<FileContentResult> GenerateFile(string path, string contentType)
        {
            var memoryStream = new MemoryStream();
            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                await fileStream.CopyToAsync(memoryStream);
            }
            return new FileContentResult(memoryStream.ToArray(), contentType);
        }
        
    }
}

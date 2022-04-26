using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoxTrackLabel.API.Models;
using BoxTrackLabel.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Serilog;

namespace BoxTrackLabel.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccessController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AccessController(IAuthService authService)
        {
            _authService = authService;
        }
        /// <summary>
        /// Método para autenticación
        /// </summary>
        /// <remarks>
        /// Ejemplo de petición:
        ///
        ///     POST /Login
        ///     {
        ///        "UserName": "Juan",
        ///        "Clave": 123
        ///     }
        ///     Retorna: Token de acceso
        /// </remarks>
        /// <param name="userInfo">Información de autenticación (usuario y clave)</param>
        /// <response code="200">Cuando se realiza la autenticación exitosamente, se obtiene un token de acceso</response>
        /// <response code="400">Fallo en la autenticación</response>
        /// <response code="401">Acceso no autorizado: el token correcto no fue proporcionado o está vencido</response>
        /// <response code="403">Acceso prohibido: el usuario no tiene privilegios para acceder al presente método</response>
        /// <response code="500">Se lanzó una excepción no controlada</response>  
        [Route("Login")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Login(LoginModel userInfo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _authService.LoginAsync(userInfo.UserName,userInfo.Clave);
                    if (result != null)
                    {
                        return Ok(result);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Contraseña y/o Usuario Incorrectos");
                        return BadRequest(ModelState);
                    }
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
        }
        /// <summary>
        /// Método para refrezcar un token expirado
        /// </summary>
        /// <remarks>
        /// Ejemplo de petición:
        ///
        ///     POST /RefreshToken
        ///     {
        ///        "expiredToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bm..."
        ///     }
        ///     Retorna: Nuevo token de acceso
        /// </remarks>
        /// <param name="tokenObject">Token expirado</param>
        /// <response code="200">Cuando se genera el nuevo token exitosamente</response>
        /// <response code="400">Fallo en la generación del nuevo token o se lanza una excepción</response>
        /// <response code="401">Acceso no autorizado: el token correcto no fue proporcionado o está vencido</response>
        /// <response code="403">Acceso prohibido: el usuario no tiene privilegios para acceder al presente método</response>
        [Route("RefreshToken")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> RefreshToken(JObject tokenObject)
        {
            try
            {
                var token = tokenObject.SelectToken("expiredToken").Value<string>();
                var result = await _authService.RefreshToken(token);
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    ModelState.AddModelError("", "Error inesperado, favor de cerrar y iniciar sesión nuevamente");
                    return BadRequest(ModelState);
                } 
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
            
        }
        // [HttpGet]
        // [AllowAnonymous]
        // public string get()
        // {
        //     return "Hola";
        // }

        protected ActionResult ManageException(Exception ex)
        {
            Log.Error(ex,ex.Message);
            ModelState.AddModelError("", "Error interno. Favor de intentarlo nuevamente.");
            return BadRequest(ModelState);
        }
    }
}
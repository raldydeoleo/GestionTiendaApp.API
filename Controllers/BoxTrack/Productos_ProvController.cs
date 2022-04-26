
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BoxTrackLabel.API.Models;
using BoxTrackLabel.API.Repositories;
using BoxTrackLabel.API.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BoxTrackLabel.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class Productos_ProvController : ControllerBase
    {
        private readonly BoxTrackDbContext _context;
        private readonly Productos_ProvRepository _repository;        

        public Productos_ProvController(Productos_ProvRepository repository) 
        {
           _repository = repository;            
        }


        [HttpPost]
        [Authorize(Permissions.Menus.Programacion)]
        public async Task<ActionResult<Productos_Prov>> CreateProductos(Productos_Prov productos_prov)
        {
            try
            {
                Productos_Prov result = null;
                result = await _repository.CreateProducto(productos_prov);
                return Created("api/listaproductos_prov", result);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }



        [Route("getallproductos_prov")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Productos_Prov>>> GetAllProductos()
        {

            return await _context.Productos_Prov.ToListAsync();
        
        }

        [Route("getall")]
        [HttpGet]
        public async Task<ActionResult<List<Productos_Prov>>> GetAll()
        {
            try
            {
                return await _repository.GetAll();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        [Route("getproducto/{id}")]
        [HttpGet]       
        public async Task<ActionResult<Productos_Prov>> GetProducto(int id)
        {
            try
            {
                Productos_Prov result = null;
                result = await  _repository.Get(id);
                if (result == null)
                {
                    return NotFound();
                }
                return result;

            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }


        [HttpPut]
        public async Task<IActionResult> Put(Productos_Prov productos_prov)
        {
            
            try
            {
                await _repository.UpdateProducto(productos_prov);
            }
            catch (System.Exception ex)
            {
                throw ex; 
            }
            return NoContent();
        }

        [HttpPut]
        [Route("delete")]
        public async Task<ActionResult<Productos_Prov>> Delete(Productos_Prov productos_prov)
        {
            try
            {                
                await _repository.Delete(productos_prov);
                return productos_prov;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }


        [HttpPost]
        [Route("getproductosporcantidad")]
        public async Task<ActionResult<List<Productos_Prov>>> GetProductosPorCantidad(ParametrosReposicion parametrosReposicion)
        {
            try
            {
                int id = parametrosReposicion.Id;
                int cantidad = parametrosReposicion.Cantidad;
                return await _repository.GetProcuctosPorCantidad(id, cantidad); 
                
            }
            catch (System.Exception ex)
            {                
                return BadRequest(ModelState);
            }            
        }

        [HttpPost]
        [Route("getproductosporfecha")]
        public async Task<ActionResult<List<Productos_Prov>>> GetProductosPorFecha(ParametrosReposicion parametrosReposicion)
        {
            try
            {
                int id = parametrosReposicion.Id;
                int cantidad = parametrosReposicion.Cantidad;
                DateTime fecha = parametrosReposicion.Fecha;                
                return await _repository.GetProcuctosPorFecha(id, cantidad, fecha);
            }
            catch (System.Exception ex)
            {
                //throw ex;
                return BadRequest(ModelState);
            }
            
        }


    }
}
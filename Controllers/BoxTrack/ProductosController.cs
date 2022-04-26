
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
    public class ProductosController : ControllerBase
    {
        private readonly BoxTrackDbContext _context;
        private readonly ProductosRepository _repository;       

    public ProductosController(ProductosRepository repository) 
        {
           _repository = repository;            
        }


        [HttpPost]
        [Authorize(Permissions.Menus.Programacion)]
        public async Task<ActionResult<Productos>> CreateProductos(Productos productos)
        {
            try
            {
                Productos result = null;
                result = await _repository.CreateProducto(productos);
                return Created("api/registrar", result);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }



        [Route("getallproductos")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Productos>>> GetAllProductos()
        {

            return await _context.Productos.ToListAsync();
        
        }

        [Route("getall")]
        [HttpGet]
        public async Task<ActionResult<List<Productos>>> GetAll()
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
        public async Task<ActionResult<Productos>> GetProducto(int id)
        {
            try
            {
                Productos result = null;
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
        public async Task<IActionResult> Put(Productos productos)
        {
            
            try
            {
                await _repository.UpdateProducto(productos);
            }
            catch (System.Exception ex)
            {
                throw ex; 
            }
            return NoContent();
        }

        [HttpPut]
        [Route("delete")]
        public async Task<ActionResult<Productos>> Delete(Productos productos)
        {
            try
            {                
                await _repository.Delete(productos);
                return productos;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }


        [HttpPost]
        [Route("updatecantidad")]        
        public async Task<ActionResult<Productos>> UpdateCantidad(ParametrosReposicion parametrosReposicion)
        {           

            try
            {
                int id = parametrosReposicion.Id;
                Productos productos = null;
                productos = await _repository.Get(id);                
                productos.Cantidad = productos.Cantidad + parametrosReposicion.Cantidad;
                await _repository.UpdateCantidadProducto(productos);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return NoContent();
        }

        [HttpPut]
        [Route("compra")]
        public async Task<ActionResult<Productos>> Compra(ParametrosCompra parametrosCompra)
        {
            try
            {                
                parametrosCompra.Fecha = DateTime.Now;
                int id = parametrosCompra.Id;
                Productos productos = null;
                productos = await _repository.Get(id);
                productos.Cantidad = productos.Cantidad - parametrosCompra.Cantidad_producto;
                await _repository.Compra(productos);
                return productos;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }

    }
}
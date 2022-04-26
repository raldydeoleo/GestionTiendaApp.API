
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
    public class SuplidoresController : ControllerBase
    {
        private readonly BoxTrackDbContext _context;
        private readonly SuplidoresRepository _repository;        

        public SuplidoresController(SuplidoresRepository repository) 
        {
           _repository = repository;            
        }


        [HttpPost]
        [Authorize(Permissions.Menus.Programacion)]
        public async Task<ActionResult<Suplidores>> CreateSuplidores(Suplidores suplidores)
        {
            try
            {
                Suplidores result = null;
                result = await _repository.CreateSuplidor(suplidores);
                return Created("api/listadesuplidores", result);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }



        [Route("getallsuplidores")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Suplidores>>> GetAllSuplidores()
        {

            return await _context.Suplidores.ToListAsync();
        
        }

        [Route("getall")]
        [HttpGet]
        public async Task<ActionResult<List<Suplidores>>> GetAll()
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

        [Route("getsuplidor/{id}")]
        [HttpGet]       
        public async Task<ActionResult<Suplidores>> GetSuplidor(int id)
        {
            try
            {
                Suplidores result = null;
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
        public async Task<IActionResult> Put(Suplidores suplidores)
        {
            
            try
            {
                await _repository.UpdateSuplidor(suplidores);
            }
            catch (System.Exception ex)
            {
                throw ex; 
            }
            return NoContent();
        }

        [HttpPut]
        [Route("delete")]
        public async Task<ActionResult<Suplidores>> Delete(Suplidores suplidores)
        {
            try
            {                
                await _repository.Delete(suplidores);
                return suplidores;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }

    }
}
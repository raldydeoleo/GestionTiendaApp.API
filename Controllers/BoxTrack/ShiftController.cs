
using System;
using System.Threading.Tasks;
using BoxTrackLabel.API.Models;
using BoxTrackLabel.API.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoxTrackLabel.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ShiftController : MyBaseController<Shift, ShiftRepository>
    {
        private readonly ShiftRepository _repository;

        public ShiftController(ShiftRepository repository) : base(repository)
        {
           _repository = repository;
        }

        [Route("getshift/{id}")]
        [HttpGet]
        public async Task<ActionResult<Shift>> Get(string id)
        {
            try
            {
                var shift = await _repository.Get(id);
                if (shift == null)
                {
                    return NotFound();
                }
                return shift;
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
        }

        [Route("getcurrentshift")]
        [HttpGet]
        public async Task<ActionResult<Shift>> GetCurrentShift()
        {
            try
            {
                var shift = await _repository.GetCurrentShift(DateTime.Now.TimeOfDay);
                if (shift == null)
                {
                    return NotFound();
                }
                return shift;
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
            
        }

        [Route("getlastshift")]
        [HttpGet]
        public async Task<ActionResult<Shift>> GetLastShift()
        {
            try
            {
                return await _repository.GetLastShift();
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
        }

    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using BoxTrackLabel.API.Models;
using BoxTrackLabel.API.Repositories;
using BoxTrackLabel.API.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BoxTrackLabel.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ModuleController : MyBaseController<Module, ModuleRepository>
    {
        private readonly ModuleRepository _repository;
        private readonly ProcessRepository processRepository;

        public ModuleController(ModuleRepository repository, ProcessRepository processRepository) : base(repository)
        {
           _repository = repository;
            this.processRepository = processRepository;
        }

       
        [Route("getallwithprocess")]
        [HttpGet]
        public async Task<ActionResult<List<Module>>> GetAllWithProcess()
        {
            try
            {
                return await _repository.GetAllWithProcess();
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
        [Route("getNextModuleCode/{processId}")]
        [HttpGet]
        public async Task<ActionResult> GetNextModuleCode(int processId)
        {
            try
            {
                var nextCode = await _repository.GetNextModuleCode(processId);
                if (nextCode == null)
                {
                    return NotFound();
                }
                return Ok(new { code = nextCode });
            }
            catch (System.Exception ex)
            {
                return ManageException(ex);
            }
            
        }
       

    }
}
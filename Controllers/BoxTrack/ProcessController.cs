
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
    public class ProcessController : MyBaseController<Process, ProcessRepository>
    {
        private readonly ProcessRepository _repository;

        public ProcessController(ProcessRepository repository) : base(repository)
        {
           _repository = repository;
        }      
    }
}
using BoxTrackLabel.API.Models;
using BoxTrackLabel.API.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoxTrackLabel.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class StorageController : MyBaseController<Storage, StorageRepository>
    {
        private readonly StorageRepository _repository;

        public StorageController(StorageRepository repository) : base(repository)
        {
            _repository = repository;
        }
    }
}

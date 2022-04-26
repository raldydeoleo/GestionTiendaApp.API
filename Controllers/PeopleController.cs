using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BoxTrackLabel.API.Models;
using BoxTrackLabel.API.Repositories;
using BoxTrackLabel.API.Utils;



namespace BoxTrackLabel.API.Controllers
{  

        [ApiController]
        public class PeopleController : ControllerBase
        {

        private readonly ProcessRepository processRepository;

        [HttpGet("people/all")]
            public async Task<ActionResult<IEnumerable<Person>>> GetAllAsync()
            {
                return new[]
                {
                    new Person { Name = "Ana" },
                    new Person { Name = "Felipe" },
                    new Person { Name = "Emillia" }
                };

            /*try
            {
                IList<Process> processes = new List<Process>() { };
                processes = await processRepository.GetAll();
                return processes;
            }
            catch (System.Exception ex)
            {
                return null;
            }*/
        }
        }

        public class Person
        {
            public string Name { get; set; }
        }
 }

      

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Udd.Api.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Udd.Api.Controllers
{
    [Route("api/cities")]
    [ApiController]
    public class CityControlle : ControllerBase
    {
        private readonly ICityService _cityService;

        public CityControlle(ICityService cityService)
        {
            _cityService = cityService;
        }

        [HttpPost("{filePath}")]
        public IActionResult Post( string filePath)
        {
            _cityService.LoadCitiesIntoDatabase(filePath);
            return Ok();
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_cityService.GetAll());
        }


    }
}

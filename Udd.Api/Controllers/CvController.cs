using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Udd.Api.Interfaces;

namespace Udd.Api.Controllers
{
    [ApiController]
    [Route("api/job-documents")]
    public class CvController : ControllerBase
    {
        private readonly ICvService _cvService;

        public CvController(ICvService cvService)
        {
            _cvService = cvService;
        }


        [HttpPost]
        public async Task<IActionResult> Reindex()
        {
            await  _cvService.IndexTestDocs();
            return Ok();
        }


        [HttpGet("personal-info")]
        public async Task<IActionResult> SearchByName([FromQuery] [Required] string name, [FromQuery][Required] string lastName)
        {
            return Ok(await _cvService.GetCvsByNameAndLastname(name, lastName));
            
        }

        [HttpGet("education")]
        public async Task<IActionResult> SearchByEducation([FromQuery][Required] int educationLevel)
        {
            return Ok(await _cvService.GetCvsByEducationLevel(educationLevel));

        }

        [HttpGet("cv-content")]
        public async Task<IActionResult> SearchByCvLetterContent([FromQuery][Required] string content)
        {
            return Ok(await _cvService.GetCvsByCvLetterContent(content));

        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _cvService.GetAll());

        }
    }
}

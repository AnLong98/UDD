using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Udd.Api.Dto;
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


        [HttpPost("reindex")]
        public async Task<IActionResult> Reindex()
        {
            await _cvService.IndexTestDocs();
            return Ok();
        }


        [HttpGet("personal-info")]
        public async Task<IActionResult> SearchByName([FromQuery][Required] string name, [FromQuery][Required] string lastName)
        {
            return Ok(await _cvService.GetCvsByNameAndLastname(name, lastName));

        }

        [HttpPost("boolean")]
        public async Task<IActionResult> BoolQuery([FromBody] CombinedQueryDto query)
        {
            return Ok(await _cvService.GetCvsCombinedQuery(query));

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

        [HttpGet("location")]
        public async Task<IActionResult> SearchByLocation([FromQuery][Required] string cityName, [FromQuery][Required] int radiusKm)
        {
            return Ok(await _cvService.GetByLocation(cityName, radiusKm));

        }

        [HttpGet("phrase")]
        public async Task<IActionResult> SearchByLocation([FromQuery][Required] string phrase)
        {
            return Ok(await _cvService.SearchAllFieldsByPhrase(phrase));

        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _cvService.GetAll());

        }

        [HttpPost]
        public async Task<IActionResult> AddNewApplication([FromForm] NewJobApplicationDto application)
        {
            bool response = await _cvService.AddNewApplication(application);
            if (response)
                return Ok();

            return BadRequest();
        }

        [HttpGet("{docID}")]
        public IActionResult GetImagesForPost(Guid docID)
        {
            var (fileType, archiveData, archiveName) = _cvService.GetJobApplicationDocsZip(docID);

            return File(archiveData, fileType, archiveName);
        }

    }
}

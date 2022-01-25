using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Udd.Api.Dto
{
    public class NewJobApplicationDto
    {
        public string ApplicantName { get; set; }
        public string ApplicantCityName { get; set; }
        public string ApplicantLastname { get; set; }
        public string CityName { get; set; }
        public int ApplicantEducationlevel { get; set; }
        public IFormFile CvFile { get; set; }
        public IFormFile CoverLetterFile { get; set; }
    }
}

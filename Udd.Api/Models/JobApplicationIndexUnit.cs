using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Udd.Api.Models
{
    public class JobApplicationIndexUnit
    {
        public Guid Id { get; set; }
        public string ApplicantName { get; set; }
        public string ApplicantLastname { get; set; }
        public int ApplicantEducationlevel { get; set; }
        public string CityName { get; set; }
        public string CvContent { get; set; }
        public string CvLetterContent { get; set; }
        public string CvFileName { get; set; }
        public string CvLetterFileName { get; set; }
        public DateTime DateCreated { get; set; }
        public GeoLocation GeoLocation { get; set; }
    }
}

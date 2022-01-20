using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Udd.Api.Dto
{
    public class JobApplicationDto
    {
        public Guid Id { get; set; }
        public string ApplicantName { get; set; }
        public string ApplicantLastname { get; set; }
        public int ApplicantEducationlevel { get; set; }
        public string CvContent { get; set; }
        public string CvLetterContent { get; set; }
        public string CvFileName { get; set; }
        public string CvLetterFileName { get; set; }
        
    }
}

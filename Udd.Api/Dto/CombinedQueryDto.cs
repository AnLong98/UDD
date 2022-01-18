using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Udd.Api.Enums;

namespace Udd.Api.Dto
{
    public class CombinedQueryDto
    {
        public string ApplicantName { get; set; }
        public string ApplicantLastName { get; set; }
        public int ApplicantEducationLevel { get; set; }
        public string CvLetterContent { get; set; }
        public QueryOperator Operator1 { get; set; }
        public QueryOperator Operator2 { get; set; }
        public QueryOperator Operator3 { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Udd.Api.Dto
{
    public class SearchResultWithHighlightsDto
    {
        public JobApplicationDto JobApplication { get; set; }
        public List<string> SearchHighlights { get; set; }
    }
}

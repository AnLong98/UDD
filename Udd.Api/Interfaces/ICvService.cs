using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Udd.Api.Dto;
using Udd.Api.Models;

namespace Udd.Api.Interfaces
{
    public interface ICvService
    {

        Task<List<JobApplicationDto>> GetCvsByNameAndLastname(string name, string lastName);
        Task<List<JobApplicationDto>> GetCvsByEducationLevel(int level);
        Task<List<SearchResultWithHighlightsDto>> GetCvsByCvLetterContent(string content);
        Task<List<JobApplicationDto>> GetCvsCombinedQuery(CombinedQueryDto query);
        Task<List<JobApplicationDto>> GetAll();
        Task<List<JobApplicationDto>> GetByLocation(string cityName, int radius);
        Task IndexTestDocs();
    }
}

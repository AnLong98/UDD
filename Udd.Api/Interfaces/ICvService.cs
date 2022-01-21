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

        Task<List<SearchResultWithHighlightsDto>> GetCvsByNameAndLastname(string name, string lastName);
        Task<List<SearchResultWithHighlightsDto>> GetCvsByEducationLevel(int level);
        Task<List<SearchResultWithHighlightsDto>> GetCvsByCvLetterContent(string content);
        Task<List<SearchResultWithHighlightsDto>> GetCvsCombinedQuery(CombinedQueryDto query);
        Task<List<SearchResultWithHighlightsDto>> GetAll();
        Task<List<SearchResultWithHighlightsDto>> GetByLocation(string cityName, int radius);
        Task IndexTestDocs();
    }
}

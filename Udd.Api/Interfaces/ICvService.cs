using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
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
        Task<List<SearchResultWithHighlightsDto>> GetCvsByCvContent(string content);
        Task<List<SearchResultWithHighlightsDto>> GetCvsCombinedQuery(CombinedQueryDto query);
        Task<List<SearchResultWithHighlightsDto>> GetAll();
        Task<List<SearchResultWithHighlightsDto>> GetByLocation(string cityName, int radius);
        Task<List<SearchResultWithHighlightsDto>> SearchAllFieldsByPhrase(string phrase);
        Task<bool> AddNewApplication(NewJobApplicationDto application);
        string ParseTextFromPdfFormFile(IFormFile file);
        FileStream GetJobApplicationDocsZip(Guid docID);
        Task<CityStats> GetCityStats();
        Task<TimeStats> GetTimeStats();
        Task IndexTestDocs();
    }
}

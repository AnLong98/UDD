using AutoMapper;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Udd.Api.Dto;
using Udd.Api.Interfaces;
using Udd.Api.Models;

namespace Udd.Api.Services
{
    public class CvService : ICvService
    {
        private readonly IElasticClient _elasticClient;
        private readonly IMapper _mapper;
        private readonly ICityService _cityService;

        public CvService(IElasticClient elasticClient, IMapper mapper, ICityService cityService)
        {
            _elasticClient = elasticClient;
            _mapper = mapper;
            _cityService = cityService;
        }

        public async Task<List<SearchResultWithHighlightsDto>> GetAll()
        {
            List<SearchResultWithHighlightsDto> results = new List<SearchResultWithHighlightsDto>();
            var response = await _elasticClient.SearchAsync<JobApplicationIndexUnit>(
            s => s.Query(q =>
                       q.MatchAll()));

            foreach (var document in response.Documents)
            {
                SearchResultWithHighlightsDto result = new SearchResultWithHighlightsDto();
                result.JobApplication = _mapper.Map<JobApplicationDto>(document);
                results.Add(result);
                result.SearchHighlights = new List<string>();
            }

            return results;
        }

        public async Task<List<SearchResultWithHighlightsDto>> GetByLocation(string cityName, int radius)
        {
            List<SearchResultWithHighlightsDto> results = new List<SearchResultWithHighlightsDto>();
            CityDto city = _cityService.GetByName(cityName);
            if (city == null)
                return null;
            var response = await _elasticClient.SearchAsync<JobApplicationIndexUnit>(
            s => s.Query(q => q
                        .GeoDistance(g => g
                        .Boost(1.1)
                        .Name("named_query")
                        .Field(p => p.GeoLocation)
                        .DistanceType(GeoDistanceType.Arc)
                        .Location(city.Latitude, city.Longitude)
                        .Distance(radius, DistanceUnit.Kilometers)
                        .ValidationMethod(GeoValidationMethod.IgnoreMalformed)
                    )));

            foreach (var document in response.Documents)
            {
                SearchResultWithHighlightsDto result = new SearchResultWithHighlightsDto();
                result.JobApplication = _mapper.Map<JobApplicationDto>(document);
                results.Add(result);
                result.SearchHighlights = new List<string>();
            }

            return results;
        }

        public async  Task<List<SearchResultWithHighlightsDto>> GetCvsByCvLetterContent(string content)
        {
            List<SearchResultWithHighlightsDto> results = new List<SearchResultWithHighlightsDto>();
            var searchResponse = await  _elasticClient.SearchAsync<JobApplicationIndexUnit>(s => s
                                                        .Query(q => q
                                                            .Bool(b => b
                                                                .Must(mu => mu
                                                                    .QueryString(m => m
                                                                        .Fields(f => f.Field(l => l.CvLetterContent))
                                                                        .Query(content)
                                                                    )
                                                                )
                                                            )
                                                        ).Highlight(h => h
                                                            .Fields(f => f
                                                                .Field(x => x.CvLetterContent)
                                                                .PreTags("<em><b>")
                                                                .PostTags("</b></em>")
                                                                )));

            var highlightsInResponse = searchResponse.Hits.Select(x => x.Highlight);
            foreach(var document in searchResponse.Documents)
            {
                SearchResultWithHighlightsDto result = new SearchResultWithHighlightsDto();
                result.JobApplication = _mapper.Map<JobApplicationDto>(document);
                result.SearchHighlights = new List<string>();
                foreach (var hit in searchResponse.Hits) // cycle through  hits to look for match
                {
                    if (hit.Id == document.Id.ToString()) //found the hit that matches document
                    {
                        foreach (var highlightField in hit.Highlight)
                        {
                            if (highlightField.Key == "cvLetterContent")
                            {
                                foreach (var highlight in highlightField.Value)
                                {
                                    result.SearchHighlights.Add(highlight);
                                }
                            }
                        }
                    }
                }
                results.Add(result);

            }


            return results;
        }

        public async Task<List<SearchResultWithHighlightsDto>> GetCvsByEducationLevel(int level)
        {
            List<SearchResultWithHighlightsDto> results = new List<SearchResultWithHighlightsDto>();
            var response = await _elasticClient.SearchAsync<JobApplicationDto>(
                s => s.Query(q =>
                               q.Term(x => x.ApplicantEducationlevel, level)));

            foreach (var document in response.Documents)
            {
                SearchResultWithHighlightsDto result = new SearchResultWithHighlightsDto();
                result.JobApplication = _mapper.Map<JobApplicationDto>(document);
                results.Add(result);
                result.SearchHighlights = new List<string>();
            }

            return results;

        }

        public async Task<List<SearchResultWithHighlightsDto>> GetCvsByNameAndLastname(string name, string lastName)
        {
            List<SearchResultWithHighlightsDto> results = new List<SearchResultWithHighlightsDto>();
            var response = await  _elasticClient.SearchAsync<JobApplicationDto>(s => s
                                                        .Query(q => q
                                                            .Bool(b => b
                                                                .Should(mu => mu
                                                                    .Match(m => m
                                                                        .Field(f => f.ApplicantName)
                                                                        .Query("*"+name+"*")
                                                                    ), mu => mu
                                                                    .Match(m => m
                                                                        .Field(f => f.ApplicantLastname)
                                                                        .Query("*"+lastName+"*")
                                                                    )
                                                                )
                                                                
                                                            )
                                                        )
                                                    );

            foreach (var document in response.Documents)
            {
                SearchResultWithHighlightsDto result = new SearchResultWithHighlightsDto();
                result.JobApplication = _mapper.Map<JobApplicationDto>(document);
                results.Add(result);
                result.SearchHighlights = new List<string>();
            }

            return results;
        }

        public async Task<List<SearchResultWithHighlightsDto>> GetCvsCombinedQuery(CombinedQueryDto query)
        {

            /*var searchRequest = new SearchRequest<JobApplicationDto>
            {
                Query = new QueryContainer(new MatchQuery { Field = Property.Path<JobApplicationDto>(p => p.Name), Query = "test" }),
            };
            */
            return null;
        }

        public async Task IndexTestDocs()
        {
            foreach(JobApplicationIndexUnit i in MockTestData.GetMockTestData())
            {
                var response = await _elasticClient.CreateDocumentAsync(i);
            }
        }
    }
}

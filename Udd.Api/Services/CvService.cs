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

        public async Task<List<JobApplicationDto>> GetAll()
        {
            var response = await _elasticClient.SearchAsync<JobApplicationIndexUnit>(
            s => s.Query(q =>
                       q.MatchAll()));

            return _mapper.Map<List<JobApplicationDto>>(response.Documents.ToList());
        }

        public async Task<List<JobApplicationDto>> GetByLocation(string cityName, int radius)
        {
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

            return _mapper.Map<List<JobApplicationDto>>(response.Documents.ToList());
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
                                                                .PreTags("<em>")
                                                                .PostTags("</em>")
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

        public async Task<List<JobApplicationDto>> GetCvsByEducationLevel(int level)
        {
            var response = await _elasticClient.SearchAsync<JobApplicationDto>(
                s => s.Query(q =>
                               q.Term(x => x.ApplicantEducationlevel, level)));
            return response.Documents.ToList();

        }

        public async Task<List<JobApplicationDto>> GetCvsByNameAndLastname(string name, string lastName)
        {
            var searchResponse = await  _elasticClient.SearchAsync<JobApplicationDto>(s => s
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

            return searchResponse.Documents.ToList();
        }

        public async Task<List<JobApplicationDto>> GetCvsCombinedQuery(CombinedQueryDto query)
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

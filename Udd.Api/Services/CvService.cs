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

        public CvService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task<List<JobApplicationIndexUnit>> GetAll()
        {
            var response = await _elasticClient.SearchAsync<JobApplicationIndexUnit>(
            s => s.Query(q =>
                       q.MatchAll()));

            return response.Documents.ToList();
        }

        public async  Task<List<JobApplicationIndexUnit>> GetCvsByCvLetterContent(string content)
        {
            var searchResponse = await  _elasticClient.SearchAsync<JobApplicationIndexUnit>(s => s
                                                        .Query(q => q
                                                            .Bool(b => b
                                                                .Should(mu => mu
                                                                    .Match(m => m
                                                                        .Field(f => f.CvLetterContent)
                                                                        .Query(content)
                                                                    )
                                                                )

                                                            )
                                                        )
                                                    );

            return searchResponse.Documents.ToList();
        }

        public async Task<List<JobApplicationIndexUnit>> GetCvsByEducationLevel(int level)
        {
            var response = await _elasticClient.SearchAsync<JobApplicationIndexUnit>(
                s => s.Query(q =>
                               q.Term(x => x.ApplicantEducationlevel, level)));
            return response.Documents.ToList();

        }

        public async Task<List<JobApplicationIndexUnit>> GetCvsByNameAndLastname(string name, string lastName)
        {
            var searchResponse = await  _elasticClient.SearchAsync<JobApplicationIndexUnit>(s => s
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

        public async Task<List<JobApplicationIndexUnit>> GetCvsCombinedQuery(CombinedQueryDto query)
        {
            var searchRequest = new SearchRequest<JobApplicationIndexUnit>
            {
                //Query = new QueryContainer(new MatchQuery { Field = Property.Path<Document>(p => p.Name), Query = "test" }),
            };

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

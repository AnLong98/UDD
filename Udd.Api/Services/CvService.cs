using AutoMapper;
using Microsoft.AspNetCore.Http;
using Nest;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
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

        public async Task<bool> AddNewApplication(NewJobApplicationDto application)
        {
            CityDto city = _cityService.GetByName(application.ApplicantCityName);

            if (city == null)
                return false;

            JobApplicationIndexUnit newApplication = _mapper.Map<JobApplicationIndexUnit>(application);
            newApplication.CvContent = ParseTextFromPdfFormFile(application.CvFile);
            newApplication.CvLetterContent = ParseTextFromPdfFormFile(application.CoverLetterFile);
            newApplication.CvFileName = application.CvFile.FileName;
            newApplication.CvLetterFileName = application.CoverLetterFile.FileName;
            newApplication.GeoLocation = new GeoLocation(city.Latitude, city.Longitude);
            newApplication.CityName = application.ApplicantCityName;
            newApplication.Id = Guid.NewGuid();
            newApplication.DateCreated = DateTime.Now;
            var response = await _elasticClient.CreateDocumentAsync(newApplication);

            if (response.IsValid)
            {
                string cvFilePath = @$"JobApplications/{newApplication.Id}-cv-{application.CvFile.FileName}";
                string letterFilePath = @$"JobApplications/{newApplication.Id}-cover-{application.CoverLetterFile.FileName}";

                //CV file save
                new FileInfo(cvFilePath).Directory?.Create();
                using (FileStream stream = new FileStream(cvFilePath, FileMode.Create))
                {
                    application.CvFile.CopyTo(stream);

                }
                //Cover letter save
                new FileInfo(letterFilePath).Directory?.Create();
                using (FileStream stream = new FileStream(letterFilePath, FileMode.Create))
                {
                    application.CoverLetterFile.CopyTo(stream);

                }
                return true;
            }


            return false;

        }

        public async Task<List<SearchResultWithHighlightsDto>> GetAll()
        {
            var searchResponse = await _elasticClient.SearchAsync<JobApplicationIndexUnit>(
            s => s.Query(q =>
                       q.MatchAll()));

            return RepackHighlightsIntoResult(searchResponse);
        }

        public async Task<List<SearchResultWithHighlightsDto>> GetByLocation(string cityName, int radius)
        {
            CityDto city = _cityService.GetByName(cityName);
            if (city == null)
                return null;
            var searchResponse = await _elasticClient.SearchAsync<JobApplicationIndexUnit>(
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

            return RepackHighlightsIntoResult(searchResponse);
        }

        public async Task<List<SearchResultWithHighlightsDto>> GetCvsByCvContent(string content)
        {
            var searchResponse = await _elasticClient.SearchAsync<JobApplicationIndexUnit>(s => s
                                                       .Query(q => q
                                                           .Bool(b => b
                                                               .Must(mu => mu
                                                                   .QueryString(m => m
                                                                       .Fields(f => f.Field(l => l.CvContent))
                                                                       .Query(content)
                                                                   )
                                                               )
                                                           )
                                                       ).Highlight(h => h
                                                           .Fields(f => f
                                                               .Field(x => x.CvContent)
                                                               .PreTags("<em><b>")
                                                               .PostTags("</b></em>")
                                                               )));

            return RepackHighlightsIntoResult(searchResponse);
        }

        public async Task<List<SearchResultWithHighlightsDto>> GetCvsByEducationLevel(int level)
        {
            var searchResponse = await _elasticClient.SearchAsync<JobApplicationIndexUnit>(
                s => s.Query(q =>
                               q.Term(x => x.ApplicantEducationlevel, level)));

            return RepackHighlightsIntoResult(searchResponse);

        }

        public async Task<List<SearchResultWithHighlightsDto>> GetCvsByNameAndLastname(string name, string lastName)
        {
            List<SearchResultWithHighlightsDto> results = new List<SearchResultWithHighlightsDto>();
            var searchResponse = await _elasticClient.SearchAsync<JobApplicationIndexUnit>(s => s
                                                       .Query(q => q
                                                           .Bool(b => b
                                                               .Should(mu => mu
                                                                   .Match(m => m
                                                                       .Field(f => f.ApplicantName)
                                                                       .Query("*" + name + "*")
                                                                   ), mu => mu
                                                                   .Match(m => m
                                                                       .Field(f => f.ApplicantLastname)
                                                                       .Query("*" + lastName + "*")
                                                                   )
                                                               )

                                                           )
                                                       )
                                                    );

            return RepackHighlightsIntoResult(searchResponse);
        }

        public async Task<List<SearchResultWithHighlightsDto>> GetCvsCombinedQuery(CombinedQueryDto query)
        {
            List<MatchQuery> queries = new List<MatchQuery>();

            MatchQuery education = new MatchQuery
            {
                Field = Nest.Infer.Field<JobApplicationIndexUnit>(p => p.ApplicantEducationlevel),
                Query = query.ApplicantEducationLevel.ToString()
            };
            queries.Add(education);

            MatchQuery name = new MatchQuery
            {
                Field = Nest.Infer.Field<JobApplicationIndexUnit>(p => p.ApplicantName),
                Query = query.ApplicantName
            };
            queries.Add(name);

            MatchQuery lastName = new MatchQuery
            {
                Field = Nest.Infer.Field<JobApplicationIndexUnit>(p => p.ApplicantLastname),
                Query = query.ApplicantLastName
            };
            queries.Add(lastName);

            MatchQuery content = new MatchQuery
            {
                Field = Nest.Infer.Field<JobApplicationIndexUnit>(p => p.CvContent),
                Query = query.CvContent
            };
            queries.Add(content);

            //Combine operators Prva izvorna verzija gde NEST prebacuje u bool query


            QueryContainer container = education;
            if (query.Operator1 == Enums.QueryOperator.AND)
            {
                container = container && name;
            }
            else
            {
                container = container || name;
            }

            if (query.Operator2 == Enums.QueryOperator.AND)
            {
                container = container && lastName;
            }
            else
            {
                container = container || lastName;
            }

            if (query.Operator3 == Enums.QueryOperator.AND)
            {
                container = container && content;
            }
            else
            {
                container = container || content;
            }


            //Druga verzija gde ja tu nesto petljam da bude preciznije/smislenije
            List<QueryContainer> must = new List<QueryContainer>();
            List<QueryContainer> should = new List<QueryContainer>();

            if (query.Operator1 == Enums.QueryOperator.AND)
            {
                must.Add(education);
                must.Add(name);
                queries.Remove(education);
                queries.Remove(name);
            }
            if (query.Operator2 == Enums.QueryOperator.AND)
            {
                if (!must.Contains(name))
                {
                    must.Add(name);
                    queries.Remove(name);
                }
                must.Add(lastName);
                queries.Remove(lastName);
            }
            if (query.Operator3 == Enums.QueryOperator.AND)
            {
                if (!must.Contains(lastName))
                {
                    must.Add(lastName);

                    queries.Remove(lastName);
                }
                must.Add(content);
                queries.Remove(content);
            }

            foreach (var queri in queries)
            {

                should.Add(queri);

            }

            //Kraj druge

            BoolQuery q = new BoolQuery
            {
                Must = must,
                Should = should
            };

            var searchResponse = await _elasticClient.SearchAsync<JobApplicationIndexUnit>(new SearchRequest<JobApplicationIndexUnit>
            {
                Query = q, //container
                Highlight = new Highlight
                {
                    PreTags = new[] { "<b>" },
                    PostTags = new[] { "</b>" },
                    Encoder = HighlighterEncoder.Html,
                    Fields = new Dictionary<Field, IHighlightField>
                    {
                        {
                            "cvContent", new HighlightField
                            {
                                Type = HighlighterType.Plain,
                                ForceSource = true,

                            }
                        }
                    }
                }
            });
            return RepackHighlightsIntoResult(searchResponse);
        }

        public async Task IndexTestDocs()
        {
            foreach (JobApplicationIndexUnit i in MockTestData.GetMockTestData())
            {
                var response = await _elasticClient.CreateDocumentAsync(i);
            }
        }

        public string ParseTextFromPdfFormFile(IFormFile file)
        {
            MemoryStream ms = new MemoryStream();
            file.OpenReadStream().CopyTo(ms);
            byte[] fileBytes = ms.ToArray();

            MemoryStream memory = new MemoryStream(fileBytes);
            StringBuilder text = new StringBuilder();


            iText.Kernel.Pdf.PdfReader iTextReader = new iText.Kernel.Pdf.PdfReader(memory);
            iText.Kernel.Pdf.PdfDocument pdfDoc = new iText.Kernel.Pdf.PdfDocument(iTextReader);

            int numberofpages = pdfDoc.GetNumberOfPages();
            for (int page = 1; page <= numberofpages; page++)
            {
                iText.Kernel.Pdf.Canvas.Parser.Listener.ITextExtractionStrategy strategy = new iText.Kernel.Pdf.Canvas.Parser.Listener.SimpleTextExtractionStrategy();
                string currentText = iText.Kernel.Pdf.Canvas.Parser.PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page), strategy);
                currentText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(
                    Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));


                text.Append(currentText);
            }
            return text.ToString();
        }


        public FileStream GetJobApplicationDocsZip(Guid docID)
        {
            var response = _elasticClient.Get<JobApplicationDto>(docID, g => g.Index("cv_index"));
            JobApplicationDto doc = response.Source;
            var zipName = $"archive-{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.zip";
            string cvFile = $"{doc.Id}-cv-{doc.CvFileName}";

            FileStream stream = new FileStream($@"JobApplications/{cvFile}", FileMode.Open);
            return stream;
        }

        public async Task<List<SearchResultWithHighlightsDto>> SearchAllFieldsByPhrase(string phrase)
        {
            var searchResponse = await _elasticClient.SearchAsync<JobApplicationIndexUnit>(s => s
                                                       .Query(q => q
                                                           .MultiMatch(mm => mm
                                                               .Fields(f => f
                                                                  .Field(x => x.ApplicantName)
                                                                  .Field(x => x.ApplicantLastname)
                                                                  .Field(x => x.CityName)
                                                                  .Field(x => x.CvContent)
                                                                  .Field(x => x.CvLetterContent)
                                                                   )
                                                               .Query(phrase)
                                                               .Type(TextQueryType.Phrase)
                                                           )
                                                       ).Highlight(h => h
                                                           .Fields(f => f
                                                               .Field("*")
                                                               .PreTags("<em><b>")
                                                               .PostTags("</b></em>")
                                                               )));

            return RepackHighlightsIntoResult(searchResponse);
        }


        private List<SearchResultWithHighlightsDto> RepackHighlightsIntoResult(ISearchResponse<JobApplicationIndexUnit> searchResponse)
        {
            List<SearchResultWithHighlightsDto> results = new List<SearchResultWithHighlightsDto>();
            foreach (var document in searchResponse.Documents)
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

                            foreach (var highlight in highlightField.Value)
                            {
                                result.SearchHighlights.Add(highlight);
                            }

                        }
                    }
                }
                results.Add(result);

            }

            return results;

        }

        public async Task<CityStats> GetCityStats()
        {
            var searchResponse = await _elasticClient.SearchAsync<JobApplicationIndexUnit>(new SearchRequest<JobApplicationIndexUnit>
            {
                Size = 0, //Da mi ne dobavlja dokumenta nego samo agregacije
                Aggregations = new AggregationDictionary
                {
                    { "most_per_city", new TermsAggregation("cities")
                        {
                            Field = Nest.Infer.Field<JobApplicationIndexUnit>(p => p.CityName),
                            Order = new List<TermsOrder>
                            {
                                TermsOrder.CountDescending
                            },

                        } 
                    },
                }

            });

            var bucketData = searchResponse.Aggregations.Terms("most_per_city").Buckets.First();
            return new CityStats
            {
                CityName = bucketData.Key,
                DocsNumber = bucketData.DocCount
            };


        }

        public async Task<TimeStats> GetTimeStats()
        {
            var searchResponse = await _elasticClient.SearchAsync<JobApplicationIndexUnit>(new SearchRequest<JobApplicationIndexUnit>
            {
                Size = 0, //Da mi ne dobavlja dokumenta nego samo agregacije
                Aggregations = new AggregationDictionary
                {
                    { "most_per_time_of_day", new AutoDateHistogramAggregation("times_of_day")
                        {
                            Field = Nest.Infer.Field<JobApplicationIndexUnit>(p => p.DateCreated),
                            Format = "a", //AM/PM
                            Buckets = 2 
                        }
                    },
                }

            });

            var bucketData = searchResponse.Aggregations.AutoDateHistogram("most_per_time_of_day").Buckets.First();
            return new TimeStats
            {
                TimeOfDay = bucketData.KeyAsString,
                DocsNumber = bucketData.DocCount
            };
        }
    }
}

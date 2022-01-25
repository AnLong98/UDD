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
            newApplication.CityName = application.CityName;
            newApplication.Id = Guid.NewGuid();
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

        public string ParseTextFromPdfFormFile(IFormFile file)
        {
            MemoryStream ms = new MemoryStream();
            file.OpenReadStream().CopyTo(ms);
            byte[] fileBytes = ms.ToArray();

            MemoryStream memory = new MemoryStream(fileBytes);
            BinaryReader BRreader = new BinaryReader(memory);
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


        public (string fileType, byte[] archiveData, string archiveName) GetJobApplicationDocsZip(Guid docID)
        {
            var response = _elasticClient.Get<JobApplicationDto>(docID, g => g.Index("cv_index"));
            JobApplicationDto doc = response.Source;

            var zipName = $"{doc.ApplicantName} {doc.ApplicantLastname}.zip";

            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {

                    var theFile = archive.CreateEntry(doc.CvFileName);
                    using (var streamWriter = new StreamWriter(theFile.Open()))
                    {
                        using (FileStream stream = new FileStream(@$"JobApplications/{doc.Id}-cv-{doc.CvFileName}", FileMode.Open))
                            streamWriter.Write(stream);
                    }

                    theFile = archive.CreateEntry(doc.CvLetterFileName);
                    using (var streamWriter = new StreamWriter(theFile.Open()))
                    {
                        using (FileStream stream = new FileStream(@$"JobApplications/{doc.Id}-cover-{doc.CvLetterFileName}", FileMode.Open))
                            streamWriter.Write(stream);
                    }


                }

                return ("application/zip", memoryStream.ToArray(), zipName);
            }
        }


    }
}

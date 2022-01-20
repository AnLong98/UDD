using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Udd.Api.Models;

namespace Udd.Api.Extensions
{
    public static class ElasticsearchExtensions
    {
        public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration["elasticsearch:url"];
            var defaultIndex = configuration["elasticsearch:index"];

            var settings = new ConnectionSettings(new Uri(url))
                .DefaultIndex(defaultIndex);

            AddDefaultMappings(settings);

            var client = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(client);

            CreateIndex(client, defaultIndex);
        }

        private static void AddDefaultMappings(ConnectionSettings settings)
        {
            settings.DefaultMappingFor<JobApplicationIndexUnit>(m => m.IdProperty(x => x.Id));
        }

        private static void CreateIndex(IElasticClient client, string indexName)
        {
            var createIndexResponse = client.Indices.Create(indexName,
                c => c
                    .Map<JobApplicationIndexUnit>(mm => mm
                    .Properties(p => p
                        .Text(t => t
                            .Name(n => n.ApplicantName)
                                .Analyzer("serbian"))
                        .Text(t => t
                            .Name(n => n.ApplicantLastname)
                                .Analyzer("serbian"))
                         .Text(t => t
                            .Name(n => n.CvContent)
                                .Analyzer("serbian"))
                        .Text(t => t
                            .Name(n => n.CvFileName)
                                .Analyzer("serbian"))
                        .Text(t => t
                            .Name(n => n.CvLetterFileName)
                                .Analyzer("serbian"))
                        .Text(t => t
                            .Name(n => n.CvLetterContent)
                                .Analyzer("serbian"))
                        .Number(t => t
                            .Name(n => n.ApplicantEducationlevel))  
                        .GeoPoint( t => t
                            .Name(n => n.GeoLocation)
                    ))));
        }
    }
}

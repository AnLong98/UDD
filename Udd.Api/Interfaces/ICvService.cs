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

        Task<List<JobApplicationIndexUnit>> GetCvsByNameAndLastname(string name, string lastName);
        Task<List<JobApplicationIndexUnit>> GetCvsByEducationLevel(int level);
        Task<List<JobApplicationIndexUnit>> GetCvsByCvLetterContent(string content);
        Task<List<JobApplicationIndexUnit>> GetCvsCombinedQuery(CombinedQueryDto query);
        Task<List<JobApplicationIndexUnit>> GetAll();
        Task IndexTestDocs();
    }
}

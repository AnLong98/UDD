using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Udd.Api.Dto;
using Udd.Api.Models;

namespace Udd.Api.Mapping
{
    public class UddMappingProfile :Profile
    {
        public UddMappingProfile()
        {
            CreateMap<JobApplicationDto, JobApplicationIndexUnit>().ReverseMap();
            CreateMap<CityDto, City>().ReverseMap();
        }
    }
}

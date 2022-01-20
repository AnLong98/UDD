using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Udd.Api.Dto;

namespace Udd.Api.Interfaces
{
    public interface ICityService
    {
        void LoadCitiesIntoDatabase(string filepath);
        CityDto GetByName(string name);
    }
}

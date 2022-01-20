using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Udd.Api.Dto;
using Udd.Api.Infrastructure;
using Udd.Api.Interfaces;
using Udd.Api.Models;

namespace Udd.Api.Services
{
    public class CityService : ICityService
    {
        private readonly IMapper _mapper;
        private readonly UddDbContext _dbContext;

        public CityService(IMapper mapper, UddDbContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public CityDto GetByName(string name)
        {
            City city = _dbContext.Cities.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
            return _mapper.Map<CityDto>(city);
        }

        public void LoadCitiesIntoDatabase(string filepath)
        {
            if (_dbContext.Cities.Count() != 0)
                return;//Dont load twice
            int i = 0;
            using (var reader = new StreamReader(filepath))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if( i == 0)
                    {
                        i++;
                        continue;
                    }    
                    string[] values = line.Split(',');

                    City newCity = new City
                    {
                        Name = values[0],
                        Latitude = double.Parse(values[1]),
                        Longitude = double.Parse(values[2])
                    };
                    _dbContext.Cities.Add(newCity);

                }
            }

            _dbContext.SaveChanges();
        }
    }
}

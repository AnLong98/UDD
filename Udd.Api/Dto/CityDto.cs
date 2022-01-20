using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Udd.Api.Dto
{
    public class CityDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}

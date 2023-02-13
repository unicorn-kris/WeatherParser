using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherParser.Service.OpenWeatherMapService.ResponseEntity
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Coordinate Coordinate { get; set; }
        public string Country { get; set; }
        public int Population { get; set; }
        public int Timezone { get; set; }
        public int Sunrise { get; set; }
        public int Sunset { get; set; }
    }
}

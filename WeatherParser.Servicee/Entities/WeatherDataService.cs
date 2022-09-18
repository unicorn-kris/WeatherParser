using System;
using System.Collections.Generic;

namespace WeatherParser.Service.Entities
{
    public class WeatherDataService
    {
        public DateTime TargetDate { get; set; }
        public List<WeatherService> Weather { get; set; }
    }
}

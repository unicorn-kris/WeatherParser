using System;
using System.Collections.Generic;

namespace WeatherParser.Repository.Entities
{
    public class WeatherDataRepository
    {
        public DateTime TargetDate { get; set; }
        public List<WeatherRepository> Weather { get; set; }
    }
}

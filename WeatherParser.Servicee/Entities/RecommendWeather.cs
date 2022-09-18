using System;
using System.Collections.Generic;

namespace WeatherParser.Servicee.Entities
{
    internal class RecommendWeather
    {
        public DateTime day;
        public List<double> Temperatures { get; set; }
        public List<int> Pressures { get; set; }
        public List<int> Humidities { get; set; }
        public List<int> WindSpeedsFirst { get; set; }
        public List<string> WindDirections { get; set; }
    }
}

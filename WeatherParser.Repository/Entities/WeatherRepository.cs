using System;
using System.Collections.Generic;

namespace WeatherParser.Repository.Entities
{
    public class WeatherRepository
    {
        //объект для сбора данных о погоде

        //ON this date i have a weather
        public DateTime Date { get; set; }
        public List<double> Temperature { get; set; }
        public List<int> Pressure { get; set; }
        public List<int> Humidity { get; set; }
        public List<int> WindSpeed { get; set; }
        public List<string> WindDirection { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace WeatherParser.Service.Entities
{
    public class WeatherService
    {
        //объект для сохранения данных о погоде
        public DateTime Date { get; set; }
        public List<double> Temperature { get; set; }
        public List<int> Pressure { get; set; }
        public List<int> Humidity { get; set; }
        public List<int> WindSpeed { get; set; }
        public List<string> WindDirection { get; set; }

    }
}

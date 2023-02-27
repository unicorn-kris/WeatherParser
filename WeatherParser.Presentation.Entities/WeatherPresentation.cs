using System;
using System.Collections.Generic;

namespace WeatherParser.Presentation.Entities
{
    public class WeatherPresentation
    {
        //объект для данных о погоде
        public DateTime Date { get; set; }
        public List<double> Temperature { get; set; }
        public List<int> Pressure { get; set; }
        public List<int> Humidity { get; set; }
        public List<double> WindSpeed { get; set; }
        public List<string> WindDirection { get; set; }

    }
}

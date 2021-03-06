using System;

namespace WeatherParser.Service.Entities
{
    public class WeatherDataService
    {
        //объект для сохранения данных о погоде
        public DateTime CollectionDate { get; set; }
        public DateTime Date { get; set; }
        public double Temperature { get; set; }
        public int Pressure { get; set; }
        public int Humidity { get; set; }
        public int WindSpeedFirst { get; set; }
        public int WindSpeedSecond { get; set; }
        public string WindDirection { get; set; }

    }
}

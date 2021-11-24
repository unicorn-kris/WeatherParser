using System;

namespace WeatherParser.Entities
{
    public class WeatherData
    {
        //объект для сохранения данных о погоде
        public DateTime Date { get; set; }
        public double Temperature { get; set; }
        public int Pressure { get; set; }
        public int Humidity { get; set; }
        public int WindSpeed_1 { get; set; }
        public int WindSpeed_2 { get; set; }
        public string WindDirection { get; set; }

    }
}

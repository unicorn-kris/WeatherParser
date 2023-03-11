using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace WeatherParser.Repository.Entities
{
    public class WeatherRepository
    {
        //объект для сбора данных о погоде

        //ON this date i have a weather
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Date { get; set; }
        public List<int> Hours { get; set; }
        public List<double> Temperature { get; set; }
        public List<double> Pressure { get; set; }
        public List<double> Humidity { get; set; }
        public List<double> WindSpeed { get; set; }
        public List<string> WindDirection { get; set; }
    }
}

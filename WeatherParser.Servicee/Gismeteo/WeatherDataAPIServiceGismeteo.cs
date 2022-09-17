using System;
using System.Collections.Generic;
using WeatherParser.Repository.Contract;
using WeatherParser.Service.Entities;
using WeatherParser.Servicee.Contract.Graphics;

namespace WeatherParser.Service
{
    public class WeatherDataAPIServiceGismeteo : IWeatherParserServiceGismeteo
    {
        private readonly IWeatherParserRepository _weatherParserRepository;

        public WeatherDataAPIServiceGismeteo(IWeatherParserRepository weatherParserRepository)
        {
            _weatherParserRepository = weatherParserRepository;
        }

        public Dictionary<DateTime, List<WeatherService>> GetAllWeatherData(DateTime targetDate)
        {
            var resultData = new Dictionary<DateTime, List<WeatherService>>();

            var weatherData = _weatherParserRepository.GetAllWeatherData(targetDate);

            //map repository entity to service entity
            foreach (var weather in weatherData)
            {
                var newListOfWeatherData = new List<WeatherService>();

                foreach (var item in weather.Value)
                {
                    newListOfWeatherData.Add(new WeatherService()
                    {
                        Temperature = item.Temperature,
                        Humidity = item.Humidity,
                        Pressure = item.Pressure,
                        WindSpeed = item.WindSpeed,
                        WindDirection = item.WindDirection,
                        Date = item.Date
                    });
                }

                resultData.Add(weather.Key, newListOfWeatherData);
            }

            return resultData;
        }

    public DateTime GetFirstDate()
    {
        return _weatherParserRepository.GetFirstDate();
    }

    public DateTime GetLastDate()
    {
        return _weatherParserRepository.GetLastDate();
    }

    public void SaveWeatherData(string url, int dayNum)
    {

    }
}
}

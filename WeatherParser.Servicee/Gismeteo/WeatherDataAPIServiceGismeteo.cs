using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<WeatherDataService> GetAllWeatherData(DateTime targetDate)
        {
            var repositoryData = _weatherParserRepository.GetAllWeatherData(targetDate);
            var resultData = new List<WeatherDataService>();

            //map repository entity to service entity
            foreach (var weather in repositoryData)
            {
                resultData.Add(new WeatherDataService()
                {
                    TargetDate = weather.TargetDate,
                    Weather = new List<WeatherService>()
                });

                foreach (var weatherData in weather.Weather)
                {
                    resultData.Last().Weather.Add(new WeatherService()
                    {
                        Date = weatherData.Date,
                        Temperature = weatherData.Temperature,
                        Humidity = weatherData.Humidity,
                        Pressure = weatherData.Pressure,
                        WindDirection = weatherData.WindDirection,
                        WindSpeed = weatherData.WindSpeed
                    });
                }
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

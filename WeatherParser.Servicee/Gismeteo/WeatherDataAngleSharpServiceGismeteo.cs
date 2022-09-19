using AngleSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using WeatherParser.Repository.Contract;
using WeatherParser.Repository.Entities;
using WeatherParser.Service.Entities;
using WeatherParser.Servicee.Contract.Graphics;

namespace WeatherParser.Service
{
    public class WeatherDataAngleSharpServiceGismeteo : IWeatherParserServiceGismeteo
    {
        private readonly IWeatherParserRepository _weatherParserRepository;

        public WeatherDataAngleSharpServiceGismeteo(IWeatherParserRepository weatherParserRepository)
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

        public void SaveWeatherData()
        {
            var config = Configuration.Default.WithDefaultLoader();
            var doc = BrowsingContext.New(config).OpenAsync("");
            var parsedHtml = doc.Result;

            //var html = parsedHtml.Body.OuterHtml;

            //File.WriteAllText("log.txt", html);

            var temperatures = parsedHtml.GetElementsByClassName("widget-row-chart widget-row-chart-temperature");
            var windSpeeds = parsedHtml.GetElementsByClassName("widget-row widget-row-wind-speed-gust row-with-caption");
            var windDirections = parsedHtml.GetElementsByClassName("widget-row widget-row-wind-direction");
            var pressures = parsedHtml.GetElementsByClassName("widget-row-chart widget-row-chart-pressure");
            var humidities = parsedHtml.GetElementsByClassName("widget-row widget-row-humidity");

            WeatherService weatherData = new WeatherService()
            {
                Temperature = new List<double>(),
                Humidity = new List<int>(),
                Pressure = new List<int>(),
                WindDirection = new List<string>(),
                WindSpeed = new List<int>()
            };

            weatherData.Date = DateTime.UtcNow.AddDays(1);

            for (int i = 0; i < 8; ++i)
            {
                string temperature = temperatures[0]
                    .GetElementsByClassName("chart")[0]
                    .GetElementsByClassName("values")[0]
                    .GetElementsByClassName("value")[i]
                    .QuerySelector("span").TextContent.Trim();

                if (temperature.Any(c => c == '−'))
                {
                    weatherData.Temperature.Add(double.Parse(temperature.Replace('−', ' ').Trim()) * -1);
                }
                else
                {
                    weatherData.Temperature.Add(double.Parse(temperature));
                }

                string windSpeed = windSpeeds[0]
                    .GetElementsByClassName("row-item")[i]
                    .QuerySelectorAll("span")[0].TextContent.Trim();

                if (windSpeed.Any(c => c == '-'))
                {
                    weatherData.WindSpeed.Add(int.Parse(windSpeed.Split('-')[0]));
                }
                else
                {
                    weatherData.WindSpeed.Add(int.Parse(windSpeed));
                }

                if (windSpeed != "0")
                {
                    weatherData.WindDirection.Add(windDirections[0]
                         .GetElementsByClassName("row-item")[i]
                         .GetElementsByClassName("direction")[0].TextContent.Trim());
                }

                weatherData.Pressure.Add(int.Parse(pressures[0]
                    .GetElementsByClassName("chart")[0]
                    .GetElementsByClassName("values")[0]
                    .GetElementsByClassName("value")[i]
                    .QuerySelectorAll("span")[0].TextContent.Trim()));

                weatherData.Humidity.Add(int.Parse(humidities[0]
                    .QuerySelectorAll("div")[i].TextContent.Trim()));
            }

            //map service entity to repository entity

            var newListOfWeatherData = new WeatherRepository()
            {
                Temperature = weatherData.Temperature,
                Humidity = weatherData.Humidity,
                Pressure = weatherData.Pressure,
                WindSpeed = weatherData.WindSpeed,
                WindDirection = weatherData.WindDirection,
                Date = weatherData.Date,
            };

            var newWeatherDataRepository = new WeatherDataRepository()
            {
                TargetDate = DateTime.UtcNow,
                Weather = new List<WeatherRepository>() { newListOfWeatherData }
            };

            _weatherParserRepository.SaveWeatherData(newWeatherDataRepository);
        }
    }
}
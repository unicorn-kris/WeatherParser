using AngleSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using WeatherParser.Contract;
using WeatherParser.Entities;
using WeatherParser.Repository.Contract;

namespace WeatherParser.Service
{
    public class WeatherDataService : IWeatherParserService
    {
        private readonly IWeatherParserRepository _weatherParserRepository;

        public WeatherDataService(IWeatherParserRepository weatherParserRepository)
        {
            _weatherParserRepository = weatherParserRepository;
        }

        public Dictionary<DateTime, List<WeatherData>> GetWeatherData(DateTime targetDate)
        {
            return _weatherParserRepository.GetWeatherData(targetDate);
        }

        public bool SaveWeatherData(string url)
        {
            List<WeatherData> listOfWeatherData = new List<WeatherData>(8);

            var config = Configuration.Default.WithDefaultLoader();
            var doc = BrowsingContext.New(config).OpenAsync(url);
            var parsedHtml = doc.Result;

            var date = parsedHtml.GetElementsByClassName("date date-2");
            var temperatures = parsedHtml.GetElementsByClassName("widget-row-chart widget-row-chart-temperature");
            var windSpeeds = parsedHtml.GetElementsByClassName("widget-row widget-row-wind-speed-gust row-with-caption");
            var windDirections = parsedHtml.GetElementsByClassName("widget-row widget-row-wind-direction");
            var pressures = parsedHtml.GetElementsByClassName("widget-row-chart widget-row-chart-pressure");
            var humidities = parsedHtml.GetElementsByClassName("widget-row widget-row-humidity");

            for (int i = 0; i < 8; ++i)
            {
                WeatherData weatherData = new WeatherData();

                weatherData.CollectionDate = DateTime.Now;

                //weatherData.Date = DateTime.Parse(date[7].GetAttribute("Title").Remove(0, date[7].GetAttribute("Title").IndexOf(',') + 5).Trim());

                string temperature = temperatures[0]
                    .GetElementsByClassName("chart")[0]
                    .GetElementsByClassName("values")[0]
                    .GetElementsByClassName("value")[i]
                    .QuerySelector("span").TextContent.Trim();

                if (temperature.Any(c => c == '−'))
                {
                    weatherData.Temperature = double.Parse(temperature.Replace('−', ' ').Trim()) * -1;
                }
                else
                {
                    weatherData.Temperature = double.Parse(temperature);
                }

                string windSpeed = windSpeeds[0]
                    .GetElementsByClassName("row-item")[i]
                    .QuerySelectorAll("span")[0].TextContent.Trim();

                if (windSpeed.Any(c => c == '-'))
                {
                    weatherData.WindSpeedFirst = int.Parse(windSpeed.Split('-')[0]);
                    weatherData.WindSpeedSecond = int.Parse(windSpeed.Split('-')[1]);
                }
                else
                {
                    weatherData.WindSpeedFirst = int.Parse(windSpeed);
                    weatherData.WindSpeedSecond = int.MaxValue;
                }

                if (windSpeed != "0")
                {
                    weatherData.WindDirection = windDirections[0]
                         .GetElementsByClassName("row-item")[i]
                         .GetElementsByClassName("direction")[0].TextContent.Trim();
                }

                weatherData.Pressure = int.Parse(pressures[0]
                    .GetElementsByClassName("chart")[0]
                    .GetElementsByClassName("values")[0]
                    .GetElementsByClassName("value")[i]
                    .QuerySelectorAll("span")[0].TextContent.Trim());

                weatherData.Humidity = int.Parse(humidities[0]
                    .QuerySelectorAll("div")[i].TextContent.Trim());

                listOfWeatherData.Add(weatherData);
            }

            return _weatherParserRepository.SaveWeatherData(listOfWeatherData);
        }

    }
}

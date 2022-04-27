using AngleSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WeatherParser.Repository.Contract;
using WeatherParser.Repository.Entities;
using WeatherParser.Service.Contract;
using WeatherParser.Service.Entities;

namespace WeatherParser.Service
{
    public class WeatherDataAngleSharpServiceGismeteo : IWeatherParserServiceGismeteo
    {
        private readonly IWeatherParserRepository _weatherParserRepository;

        public WeatherDataAngleSharpServiceGismeteo(IWeatherParserRepository weatherParserRepository)
        {
            _weatherParserRepository = weatherParserRepository;
        }

        public Dictionary<DateTime, List<WeatherDataService>> GetAllWeatherData(DateTime targetDate)
        {
            var resultData = new Dictionary<DateTime, List<WeatherDataService>>();

            var weatherData = _weatherParserRepository.GetAllWeatherData(targetDate);

            //map repository entity to service entity
            foreach (var weather in weatherData)
            {
                var newListOfWeatherData = new List<WeatherDataService>();

                foreach (var item in weather.Value)
                {
                    newListOfWeatherData.Add(new WeatherDataService()
                    {
                        Temperature = item.Temperature,
                        Humidity = item.Humidity,
                        Pressure = item.Pressure,
                        WindSpeedFirst = item.WindSpeedFirst,
                        WindSpeedSecond = item.WindSpeedSecond,
                        WindDirection = item.WindDirection,
                        CollectionDate = item.CollectionDate,
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
            List<WeatherDataService> listOfWeatherData = new List<WeatherDataService>(8);

            var config = Configuration.Default.WithDefaultLoader();
            var doc = BrowsingContext.New(config).OpenAsync(url);
            var parsedHtml = doc.Result;

            var html = parsedHtml.Body.OuterHtml;

            File.WriteAllText("log.txt", html);

            var temperatures = parsedHtml.GetElementsByClassName("widget-row-chart widget-row-chart-temperature");
            var windSpeeds = parsedHtml.GetElementsByClassName("widget-row widget-row-wind-speed-gust row-with-caption");
            var windDirections = parsedHtml.GetElementsByClassName("widget-row widget-row-wind-direction");
            var pressures = parsedHtml.GetElementsByClassName("widget-row-chart widget-row-chart-pressure");
            var humidities = parsedHtml.GetElementsByClassName("widget-row widget-row-humidity");

            for (int i = 0; i < 8; ++i)
            {
                WeatherDataService weatherData = new WeatherDataService();

                weatherData.CollectionDate = DateTime.Now;

                weatherData.Date = weatherData.CollectionDate.AddDays(dayNum);

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

            //map service entity to repository entity
            var newListOfWeatherData = new List<WeatherDataRepository>();

            foreach (var weatherData in listOfWeatherData)
            {
                newListOfWeatherData.Add(new WeatherDataRepository()
                {
                    Temperature = weatherData.Temperature,
                    Humidity = weatherData.Humidity,
                    Pressure = weatherData.Pressure,
                    WindSpeedFirst = weatherData.WindSpeedFirst,
                    WindSpeedSecond = weatherData.WindSpeedSecond,
                    WindDirection = weatherData.WindDirection,
                    CollectionDate = weatherData.CollectionDate,
                    Date = weatherData.Date,
                });
            }

            _weatherParserRepository.SaveWeatherData(newListOfWeatherData);
        }
    }
}

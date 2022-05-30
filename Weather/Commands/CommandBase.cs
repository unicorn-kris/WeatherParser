using System;
using System.Collections.Generic;
using WeatherParser.GrpcService.Services;
using WeatherParser.Presentation.Entities;

namespace WeatherParser.WPF.Commands
{
    internal abstract class CommandBase
    {
        public Dictionary<DateTime, List<WeatherDataPresentation>> GetResponseToDictionary(WeatherDataGetResponse weatherDataGetResponse)
        {
            var result = new Dictionary<DateTime, List<WeatherDataPresentation>>();

            foreach (var item in weatherDataGetResponse.WeatherDataDictionary)
            {
                var weatherDataList = new List<WeatherDataPresentation>();

                foreach (var weatherData in item.Value.WeatherDataList)
                {
                    weatherDataList.Add(new WeatherDataPresentation()
                    {
                        CollectionDate = weatherData.CollectionDate.ToDateTime(),
                        Date = weatherData.Date.ToDateTime(),
                        Temperature = weatherData.Temperature,
                        Humidity = weatherData.Humidity,
                        Pressure = weatherData.Pressure,
                        WindDirection = weatherData.WindDirection,
                        WindSpeedFirst = weatherData.WindSpeedFirst,
                        WindSpeedSecond = weatherData.WindSpeedSecond
                    });
                }

                if (!result.ContainsKey(item.Key.ToDateTime()))
                {
                    result.Add(item.Key.ToDateTime(), weatherDataList);
                }
            }
            return result;
        }
    }
}

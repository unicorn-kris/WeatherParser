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
                    var temps = new List<double>();
                    foreach (var temp in weatherData.Temperatures.Temperature)
                    {
                        temps.Add(temp);
                    }

                    var hums = new List<int>();
                    foreach (var hum in weatherData.Humidities.Humidity)
                    {
                        hums.Add(hum);
                    }

                    var press = new List<int>();
                    foreach (var pres in weatherData.Pressures.Pressure)
                    {
                        press.Add(pres);
                    }

                    var windDirs = new List<string>();
                    foreach (var windDir in weatherData.WindDirections.WindDirection)
                    {
                        windDirs.Add(windDir);
                    }

                    var windSpeeds = new List<int>();
                    foreach (var windSpeed in weatherData.WindSpeeds.WindSpeed)
                    {
                        windSpeeds.Add(windSpeed);
                    }

                    weatherDataList.Add(new WeatherDataPresentation()
                    {
                        Date = weatherData.Date.ToDateTime(),
                        Temperature = temps,
                        Humidity = hums,
                        Pressure = press,
                        WindDirection = windDirs,
                        WindSpeed = windSpeeds
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

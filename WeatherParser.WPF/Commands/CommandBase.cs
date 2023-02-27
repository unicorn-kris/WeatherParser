using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WeatherParser.GrpcService.Services;
using WeatherParser.Presentation.Entities;

namespace WeatherParser.WPF.Commands
{
    internal abstract class CommandBase
    {
        public List<WeatherDataPresentation> GetLabelsAndResponse(
            WeatherDataGetResponse weatherDataGetResponse,
            ObservableCollection<Axis> XAxes,
            DateTime selectedDate)
        {
            if (selectedDate != null)
            {
                XAxes.Clear();

                XAxes.Add(new Axis()
                {
                    Labels = weatherDataGetResponse.WeatherData.Select(s => s.TargetDate.ToDateTime().ToString("dd.MM.yyyy")).ToList(),
                    LabelsPaint = new SolidColorPaintTask(SKColors.Black)
                });
            }

            var result = new List<WeatherDataPresentation>();

            foreach (var item in weatherDataGetResponse.WeatherData)
            {
                var weatherDataList = new List<WeatherPresentation>();

                foreach (var weatherData in item.Weather.WeatherList)
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

                    var windSpeeds = new List<double>();
                    foreach (var windSpeed in weatherData.WindSpeeds.WindSpeed)
                    {
                        windSpeeds.Add(windSpeed);
                    }

                    weatherDataList.Add(new WeatherPresentation()
                    {
                        Date = weatherData.Date.ToDateTime(),
                        Temperature = temps,
                        Humidity = hums,
                        Pressure = press,
                        WindDirection = windDirs,
                        WindSpeed = windSpeeds
                    });
                }

                result.Add(new WeatherDataPresentation() { TargetDate = item.TargetDate.ToDateTime(), Weather = weatherDataList });
            }
            return result;
        }
    }
}

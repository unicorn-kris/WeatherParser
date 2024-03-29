﻿using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WeatherParser.Presentation.Entities;
using WeatherParser.WPF.ViewModels;

namespace WeatherParser.WPF.Commands
{
    internal abstract class WeatherCommandBase: IWeatherCommand
    {
        public void Execute(List<WeatherDataPresentation> weatherDataList,
            ObservableCollection<ISeries> series,
            ObservableCollection<TimeViewModel> times)
        {
            if (weatherDataList != null)
            {
                for (int i = 0; i < times.Count; ++i)
                {
                    if (times[i].IsChecked)
                    {
                        var values = new List<WeatherSample>();

                        foreach (var weatherData in weatherDataList)
                        {
                            foreach (var weather in weatherData.Weather)
                            {
                                if (weather.Hours.Contains(times[i].CurrentTime) && weather.Temperature.Any())
                                {
                                    values.Add(new WeatherSample()
                                    {
                                        Value = Math.Round(AddData(weather, i), 2),
                                        Ticks = weatherData.TargetDate.Ticks
                                    });
                                }
                                else
                                {
                                    values.Add(new WeatherSample()
                                    {
                                        Value = null,
                                        Ticks = weatherData.TargetDate.Ticks
                                    });
                                }
                            }
                        }
                        series.Add(new LineSeries<WeatherSample>
                        {
                            Mapping = (x, y) =>
                            {
                                if (x.Value == null)
                                {
                                    y.IsNull = true;
                                }
                                else
                                {
                                    y.PrimaryValue = (double)x.Value;
                                }
                                y.SecondaryValue = x.Ticks;
                            },
                            Values = values,
                            Name = $"{times[i].CurrentTime}.00"
                        });
                    }
                }
            }
        }

        public virtual double AddData(WeatherPresentation weatherPresentation, int index)
        {
            return double.NaN;
        }
    }
}

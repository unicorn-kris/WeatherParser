using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WeatherParser.Presentation.Entities;
using WeatherParser.WPF.ViewModels;

namespace WeatherParser.WPF.Commands
{
    internal abstract class CommandBase
    {
        public void CreateSeries(List<WeatherDataPresentation> weatherDataList,
            ObservableCollection<TimeViewModel> times,
            ObservableCollection<ISeries> series,
            ObservableCollection<Axis> xAxes,
            DateTime? selectedDate)
        {
            if (selectedDate != null)
            {
                xAxes[0].Labels.Clear();

                foreach (var date in weatherDataList.Select(s => s.TargetDate.ToShortDateString()))
                {
                    xAxes[0].Labels.Add(date);
                }
            }

            var dates = xAxes[0].Labels.ToList();

            if (weatherDataList != null)
            {
                for (int i = 0; i < times.Count; ++i)
                {
                    int j = 0;

                    if (times[i].IsChecked)
                    {
                        var values = new List<double?>();

                        foreach (var weatherData in weatherDataList)
                        {
                            foreach (var weather in weatherData.Weather)
                            {
                                if (weather.Hours.Count > i && weather.Hours.Contains(times[i].CurrentTime) && weather.Temperature.Any())
                                {
                                    if (weatherData.TargetDate.Date == DateTime.Parse(dates[j]).Date)
                                    {
                                        values.Add(Math.Round(AddData(weather, i), 2));
                                    }
                                }
                                else
                                {
                                    values.Add(null);
                                }
                                ++j;
                            }
                        }
                        series.Add(new LineSeries<double?> { Values = values, Name = $"{times[i].CurrentTime}.00" });
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

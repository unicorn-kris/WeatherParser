using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WeatherParser.GrpcService.Services;
using WeatherParser.Presentation.Entities;
using WeatherParser.WPF.ViewModels;

namespace WeatherParser.WPF.Commands
{
    internal class GetHumidityCommand : CommandBase, ICommand
    {
        ILogger _logger;

        public GetHumidityCommand(ILogger logger)
        {
            _logger = logger;
        }

        public void Execute(WeatherDataGetResponse weatherDataGetResponse,
             DateTime? selectedDate,
             ObservableCollection<ISeries> series,
             SitePresentation selectedSite,
             ObservableCollection<TimeViewModel> times,
             ObservableCollection<Axis> xAxes)
        {
            series.Clear();

            List<WeatherDataPresentation> weatherData = null;

            try
            {
                weatherData = GetLabelsAndResponse(
                    weatherDataGetResponse,
                    xAxes,
                    (DateTime)selectedDate);
            }
            catch (Exception ex)
            {
                _logger.Error($"{this.GetType().Name} have an exception with message: {ex.Message}");
            }

            var dates = xAxes[0].Labels.ToList();

            if (weatherData != null)
            {
                for (int i = 0; i < times.Count; ++i)
                {
                    int j = 0;

                    if (times[i].IsChecked)
                    {
                        var humValues = new List<double?>();

                        foreach (var weather in weatherData)
                        {
                            foreach (var hum in weather.Weather)
                            {
                                if (hum.Hours.Count > i && hum.Hours.Contains(times[i].CurrentTime))
                                {
                                    if (weather.TargetDate.Date == DateTime.Parse(dates[j]).Date)
                                    {
                                        humValues.Add(Math.Round(hum.Temperature[i]));
                                    }
                                }
                                else
                                {
                                    humValues.Add(null);
                                }
                                ++j;
                            }
                        }
                        series.Add(new LineSeries<double?> { Values = humValues, Name = $"{times[i].CurrentTime}.00" });
                    }
                }
            }
        }
    }
}

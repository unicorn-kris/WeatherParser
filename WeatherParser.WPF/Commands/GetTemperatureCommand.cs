using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WeatherParser.GrpcService.Services;
using WeatherParser.Presentation.Entities;
using WeatherParser.WPF.ViewModels;

namespace WeatherParser.WPF.Commands
{
    internal class GetTemperatureCommand : CommandBase, ICommand
    {
        ILogger _logger;

        public GetTemperatureCommand(ILogger logger)
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
                     weatherDataGetResponse);
            }
            catch (Exception ex)
            {
                _logger.Error($"{this.GetType().Name} have an exception with message: {ex.Message}");
            }

            if (weatherData != null)
            {
                for (int i = 0; i < times.Count; ++i)
                {
                    if (times[i].IsChecked)
                    {
                        var tempValues = new List<DateTimePoint>();

                        foreach (var weather in weatherData)
                        {
                            foreach (var temp in weather.Weather)
                            {
                                tempValues.Add(new DateTimePoint() { DateTime = temp.Date, Value = temp.Temperature[i] });
                            }
                        }
                        series.Add(new LineSeries<DateTimePoint> { Values = tempValues, Name = $"{times[i].CurrentTime}.00" });
                    }
                }
            }
        }
    }
}

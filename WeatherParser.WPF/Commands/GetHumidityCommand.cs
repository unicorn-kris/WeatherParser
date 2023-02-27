using Google.Protobuf.WellKnownTypes;
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
                        var humValues = new List<DateTimePoint>();

                        foreach (var weather in weatherData)
                        {
                            foreach (var hum in weather.Weather)
                            {
                                humValues.Add(new DateTimePoint() { DateTime = hum.Date, Value = hum.Humidity[i] });
                            }
                        }
                        series.Add(new LineSeries<DateTimePoint> { Values = humValues, Name = $"{times[i].CurrentTime}.00" });
                    }
                }
            }
        }
    }
}

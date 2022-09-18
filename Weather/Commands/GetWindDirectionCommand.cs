using Google.Protobuf.WellKnownTypes;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WeatherParser.GrpcService.Services;
using WeatherParser.Presentation.Entities.Entities;
using WeatherParser.WPF.ViewModels;

namespace WeatherParser.WPF.Commands
{
    internal class GetWindDirectionCommand : CommandBase, ICommand
    {
        ILogger _logger;

        public GetWindDirectionCommand(ILogger logger)
        {
            _logger = logger;
        }

        public void Execute(WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient weatherParserService,
            DateTime? selectedDate,
            ObservableCollection<ISeries> Series,
            ObservableCollection<TimeViewModel> Times,
            ObservableCollection<Axis> XAxes)
        {
            Series.Clear();

            List<WeatherDataPresentation> weatherData = null;

            try
            {
                weatherData = GetLabelsAndResponse(weatherParserService.GetAllWeatherData(DateTime.SpecifyKind((DateTime)selectedDate, DateTimeKind.Utc).ToTimestamp()), XAxes, Times, (DateTime)selectedDate);
            }
            catch (Exception ex)
            {
                _logger.Error($"{this.GetType().Name} have an exception with message: {ex.Message}");
            }

            if (weatherData != null)
            {
                for (int i = 0; i < Times.Count; ++i)
                {
                    if (Times[i].IsChecked)
                    {
                        var windDirValues = new List<double>();

                        foreach (var weather in weatherData)
                        {
                            foreach (var windDir in weather.Weather)
                            {
                                windDirValues.Add(windDir.Humidity[i]);
                            }
                        }
                        Series.Add(new LineSeries<double> { Values = windDirValues, Name = $"{Times[i].CurrentTime}.00" });
                    }
                }
            }
        }
    }
}

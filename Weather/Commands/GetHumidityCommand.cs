using Google.Protobuf.WellKnownTypes;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public Dictionary<DateTime, List<WeatherDataPresentation>> GetResponseToDictionary(WeatherDataGetResponse weatherDataGetResponse) => base.GetResponseToDictionary(weatherDataGetResponse);

        public void Execute(WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient weatherParserService,
            DateTime? selectedDate,
            ObservableCollection<ISeries> Series,
            ObservableCollection<TimeViewModel> Times)
        {
            Series.Clear();

            Dictionary<DateTime, List<WeatherDataPresentation>> weatherData = null;

            try
            {
                weatherData = GetResponseToDictionary(weatherParserService.GetAllWeatherData(selectedDate.Value.ToUniversalTime().ToTimestamp()));
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
                        var tempValues = new List<double>();

                        foreach (var temp in weatherData.Values)
                        {
                            tempValues.Add(temp[i].Humidity);
                        }
                        Series.Add(new LineSeries<double> { Values = tempValues, Name = $"{Times[i].CurrentTime}.00" });
                    }
                }
            }
        }
    }
}

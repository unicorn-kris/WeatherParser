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
    internal class GetWindDirectionCommand : ICommand
    {
        ILogger _logger;

        public GetWindDirectionCommand(ILogger logger)
        {
            _logger = logger;
        }

        public override void Execute(WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient weatherParserService,
            Dictionary<DateTime, List<WeatherDataPresentation>> weatherData,
            DateTime? selectedDate,
            ObservableCollection<ISeries> Series,
            ObservableCollection<TimeViewModel> Times)
        {
            Series.Clear();

            weatherData = GetResponseToDictionary(weatherParserService.GetAllWeatherData(selectedDate.Value.ToUniversalTime().ToTimestamp()));

            if (weatherData != null)
            {
                for (int i = 0; i < Times.Count; ++i)
                {
                    if (Times[i].IsChecked)
                    {
                        var tempValues = new List<string>();

                        foreach (var temp in weatherData.Values)
                        {
                            tempValues.Add(temp[i].WindDirection);
                        }
                        Series.Add(new LineSeries<string> { Values = tempValues, Name = $"{Times[i].CurrentTime}.00" });
                        //Series.Add(new LineSeries<double> { Values = new List<double>() { 1, 2, 3}, Name = $"{Times[i].CurrentTime}.00"});
                    }
                }
            }
        }
    }
}

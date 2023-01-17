﻿using Google.Protobuf.WellKnownTypes;
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
    internal class GetPressureCommand : CommandBase, ICommand
    {
        ILogger _logger;

        public GetPressureCommand(ILogger logger)
        {
            _logger = logger;
        }

        public void Execute(WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient weatherParserService,
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
                    weatherParserService.GetAllWeatherData(new WeatherDataRequest() { 
                        Date = DateTime.SpecifyKind((DateTime)selectedDate, DateTimeKind.Utc).ToTimestamp(), 
                        SiteID = selectedSite.ID.ToString() }),
                    xAxes,
                    (DateTime)selectedDate);
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
                        var presValues = new List<double>();

                        foreach (var weather in weatherData)
                        {
                            foreach (var pres in weather.Weather)
                            {
                                presValues.Add(pres.Humidity[i]);
                            }
                        }
                        series.Add(new LineSeries<double> { Values = presValues, Name = $"{times[i].CurrentTime}.00" });
                    }
                }
            }
        }
    }
}
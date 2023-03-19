﻿using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WeatherParser.Presentation.Entities;
using WeatherParser.WPF.ViewModels;

namespace WeatherParser.WPF.Commands
{
    internal class GetWindSpeedCommand : CommandBase, ICommand
    {
        ILogger _logger;
        public GetWindSpeedCommand(ILogger logger)
        {
            _logger = logger;
        }

        public void Execute(List<WeatherDataPresentation> weatherDataList,
             DateTime? selectedDate,
             ObservableCollection<ISeries> series,
             ObservableCollection<TimeViewModel> times,
             ObservableCollection<Axis> xAxes)
        {
            try
            {
                CreateSeries(weatherDataList,
                    times,
                    series,
                    xAxes,
                    selectedDate);
            }
            catch (Exception ex)
            {
                _logger.Error($"{this.GetType().Name} have an exception with message: {ex.Message}");
            }

        }

        public override double AddData(WeatherPresentation weatherPresentation, int index)
        {
            return weatherPresentation.WindSpeed[index];
        }
    }
}

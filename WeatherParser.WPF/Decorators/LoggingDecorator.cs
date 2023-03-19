using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WeatherParser.Presentation.Entities;
using WeatherParser.WPF.Commands;
using WeatherParser.WPF.ViewModels;

namespace WeatherParser.WPF.Decorators
{
    internal class LoggingDecorator : BaseCommandDecorator
    {
        public LoggingDecorator(ILogger logger, ICommand command) : base(logger, command)
        { }

        public override void Execute(List<WeatherDataPresentation> weatherDataList,
             DateTime? selectedDate,
             ObservableCollection<ISeries> series,
             ObservableCollection<TimeViewModel> times,
             ObservableCollection<Axis> xAxes)
        {
            _logger.Information($"{_command.GetType().Name} started");

            _command.Execute(weatherDataList, selectedDate, series, times, xAxes);

            _logger.Information($"{_command.GetType().Name} finished");
        }
    }
}

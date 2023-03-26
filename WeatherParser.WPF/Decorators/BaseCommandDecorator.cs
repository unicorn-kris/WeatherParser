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
    internal abstract class BaseCommandDecorator : IWeatherCommand
    {
        protected IWeatherCommand _command;
        protected ILogger _logger;

        public BaseCommandDecorator(ILogger logger, IWeatherCommand command)
        {
            _command = command;
            _logger = logger;
        }

        public abstract void Execute(List<WeatherDataPresentation> weatherDataList,
            ObservableCollection<ISeries> series,
            ObservableCollection<TimeViewModel> times);

    }
}

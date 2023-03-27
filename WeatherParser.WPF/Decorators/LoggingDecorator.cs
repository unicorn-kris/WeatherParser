using LiveChartsCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WeatherParser.Presentation.Entities;
using WeatherParser.WPF.Commands;
using WeatherParser.WPF.ViewModels;

namespace WeatherParser.WPF.Decorators
{
    internal class LoggingDecorator : IWeatherCommand
    {
        protected IWeatherCommand _command;
        protected ILogger _logger;

        public LoggingDecorator(ILogger logger, IWeatherCommand command)
        {
            _command = command;
            _logger = logger;
        }

        public void Execute(List<WeatherDataPresentation> weatherDataList,
             ObservableCollection<ISeries> series,
             ObservableCollection<TimeViewModel> times)
        {
            _logger.Information($"{_command.GetType().Name} started");

            try
            {
                _command.Execute(weatherDataList, series, times);
            }
            catch (Exception ex)
            {
                _logger.Error($"{_command.GetType().Name} have an exception with message: {ex.Message}");
            }

            _logger.Information($"{_command.GetType().Name} finished");
        }
    }
}

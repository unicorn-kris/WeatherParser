using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Serilog;
using System;
using System.Collections.ObjectModel;
using WeatherParser.GrpcService.Services;
using WeatherParser.WPF.Commands;
using WeatherParser.WPF.ViewModels;

namespace WeatherParser.WPF.Decorators
{
    internal class LoggingDecorator : BaseCommandDecorator
    {
        public LoggingDecorator(ILogger logger, ICommand command) : base(logger, command)
        { }

        public override void Execute(WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient weatherParserService, DateTime? selectedDate, ObservableCollection<ISeries> Series, ObservableCollection<TimeViewModel> Times, ObservableCollection<Axis> XAxes)
        {
            _logger.Information($"{_command.GetType().Name} started");

            _command.Execute(weatherParserService, selectedDate, Series, Times, XAxes);

            _logger.Information($"{_command.GetType().Name} finished");
        }
    }
}

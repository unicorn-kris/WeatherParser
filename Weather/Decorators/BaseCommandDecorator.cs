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
    internal abstract class BaseCommandDecorator : ICommand
    {
        protected ICommand _command;
        protected ILogger _logger;

        public BaseCommandDecorator(ILogger logger, ICommand command)
        {
            _command = command;
            _logger = logger;
        }
        public abstract void Execute(WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient weatherParserService, 
            DateTime? selectedDate, 
            ObservableCollection<ISeries> Series, 
            ObservableCollection<TimeViewModel> Times, 
            ObservableCollection<Axis> XAxes);
    }
}

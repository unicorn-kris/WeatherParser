using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WeatherParser.GrpcService.Services;
using WeatherParser.Presentation.Entities;
using WeatherParser.WPF.Commands;
using WeatherParser.WPF.ViewModels;

namespace WeatherParser.WPF.Decorators
{
    internal class LoggingDecorator : BaseCommandDecorator
    {
        public LoggingDecorator(ILogger logger, ICommand command) : base(logger, command)
        { }

        public override async Task ExecuteAsync(WeatherDataGetResponse weatherDataGetResponse,
             DateTime? selectedDate,
             ObservableCollection<ISeries> series,
             SitePresentation selectedSite,
             ObservableCollection<TimeViewModel> times,
             ObservableCollection<Axis> xAxes)
        {
            _logger.Information($"{_command.GetType().Name} started");

            await _command.ExecuteAsync(weatherDataGetResponse, selectedDate, series, selectedSite, times, xAxes);

            _logger.Information($"{_command.GetType().Name} finished");
        }
    }
}

using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WeatherParser.Presentation.Entities;
using WeatherParser.WPF.ViewModels;

namespace WeatherParser.WPF.Commands
{
    internal class GetWindSpeedCommand : WeatherCommandBase, IWeatherCommand
    {
        ILogger _logger;
        public GetWindSpeedCommand(ILogger logger)
        {
            _logger = logger;
        }

        public void Execute(List<WeatherDataPresentation> weatherDataList,
             ObservableCollection<ISeries> series,
             ObservableCollection<TimeViewModel> times)
        {
            try
            {
                CreateSeries(weatherDataList,
                    times,
                    series);
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

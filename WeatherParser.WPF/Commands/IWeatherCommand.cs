using LiveChartsCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WeatherParser.Presentation.Entities;
using WeatherParser.WPF.ViewModels;

namespace WeatherParser.WPF.Commands
{
    internal interface IWeatherCommand
    {
        void Execute(List<WeatherDataPresentation> weatherDataList,
             ObservableCollection<ISeries> series,
             ObservableCollection<TimeViewModel> times);
    }
}

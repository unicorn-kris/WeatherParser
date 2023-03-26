using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using WeatherParser.WPF.Commands;

namespace WeatherParser.WPF.ViewModels.Contract
{
    internal interface IDeviationsViewModel : INotifyPropertyChanged
    {
        void ExecuteCommand(IWeatherCommand command, ObservableCollection<TimeViewModel> times);
    }
}

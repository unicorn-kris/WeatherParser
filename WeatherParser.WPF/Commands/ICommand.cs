using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WeatherParser.Presentation.Entities;
using WeatherParser.WPF.ViewModels;

namespace WeatherParser.WPF.Commands
{
    internal interface ICommand
    {
        void Execute(List<WeatherDataPresentation> weatherDataList,
             ObservableCollection<ISeries> series,
             ObservableCollection<TimeViewModel> times);
    }
}

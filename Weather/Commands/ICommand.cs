using LiveChartsCore;
using System;
using System.Collections.ObjectModel;
using WeatherParser.GrpcService.Services;
using WeatherParser.WPF.ViewModels;

namespace WeatherParser.WPF.Commands
{
    internal interface ICommand
    {
        void Execute(WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient weatherParserService,
             DateTime? selectedDate,
             ObservableCollection<ISeries> Series,
             ObservableCollection<TimeViewModel> Times);
    }
}

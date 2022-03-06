using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace WeatherParser.WPF.ViewModels
{
    internal class MainWindowViewModel : NotifyPropertyChangedBase, IMainWindowViewModel
    {
        private DateTime? _selectedDate;

        public MainWindowViewModel()
        {
            Series = new ObservableCollection<ISeries>();
            XAxes = new ObservableCollection<Axis>();
            YAxes = new ObservableCollection<Axis>();
            TemperatureCommand = new RelayCommand(Temperature);
            PressureCommand = new RelayCommand(Pressure);
        }

        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set => OnPropertyChanged(value, ref _selectedDate);
        }

        public ObservableCollection<Axis> XAxes { get; }

        public ObservableCollection<Axis> YAxes { get; }

        public ObservableCollection<ISeries> Series { get; }

        public ICommand TemperatureCommand { get; }

        public ICommand PressureCommand { get; }

        public void Temperature(object? parameter)
        {

        }

        public void Pressure(object? parameter)
        {

        }

    }
}

using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using WeatherParser.Contract;
using WeatherParser.Entities;

namespace WeatherParser.WPF.ViewModels
{
    internal class MainWindowViewModel : NotifyPropertyChangedBase, IMainWindowViewModel
    {
        #region fields

        private DateTime? _selectedDate;

        private bool _isTimeSelected = false;

        private IWeatherParserService _weatherParserService;

        private Dictionary<DateTime, List<WeatherData>> _weatherData;

        private DateTime _firstDate;

        private DateTime _lastDate;

        #endregion

        #region ctor

        public MainWindowViewModel(IWeatherParserService weatherParserService)
        {
            Series = new ObservableCollection<ISeries>();
            XAxes = new ObservableCollection<Axis>();
            YAxes = new ObservableCollection<Axis>() { new Axis() };
            Times = new ObservableCollection<TimeViewModel>();

            for (int i = 0; i < 8; ++i)
            {
                if (i == 0)
                {
                    Times.Add(new TimeViewModel { CurrentTime = 1 });
                }
                else
                {
                    Times.Add(new TimeViewModel { CurrentTime = Times[i - 1].CurrentTime + 3 });
                }

                Times[i].IsChecked = false;
                Times[i].IsDateChecked = false;

                //subscribe mainViewModel on ischecked property change for change _isTimeSelected
                PropertyChangedEventManager.AddHandler(Times[i], OnTimeChecked, nameof(TimeViewModel.IsChecked));
            }

            TemperatureCommand = new RelayCommand(Temperature);
            PressureCommand = new RelayCommand(Pressure);
            HumidityCommand = new RelayCommand(Humidity);
            //WindDirectionCommand = new RelayCommand(WindDirection);
            WindSpeedCommand = new RelayCommand(WindSpeed);

            _weatherParserService = weatherParserService;

            FirstDate = _weatherParserService.GetFirstDate();
            LastDate = _weatherParserService.GetLastDate();
        }

        private void OnTimeChecked(object? sender, PropertyChangedEventArgs e)
        {
            IsTimeSelected = Times.Any(t => t.IsChecked);
        }

        #endregion

        #region props

        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                OnPropertyChanged(value, ref _selectedDate);

                if (_selectedDate != null)
                {
                    _weatherData = _weatherParserService.GetAllWeatherData(_selectedDate.Value);
                    for (int i = 0; i < Times.Count; ++i)
                    {
                        Times[i].IsDateChecked = true;
                    }

                    XAxes.Clear();

                    XAxes.Add(new Axis()
                    {
                        Labels = _weatherData.Keys.Select(s => s.Date.ToString()).ToList(),
                        //Labels = new List<string>() { "1", "2", "3" },
                        LabelsPaint = new SolidColorPaintTask(SKColors.Black)
                    });
                }
            }
        }

        public DateTime FirstDate
        {
            get => _firstDate;

            set => OnPropertyChanged(value, ref _firstDate);
        }

        public DateTime LastDate
        {
            get => _lastDate;

            set => OnPropertyChanged(value, ref _lastDate);
        }

        public bool IsTimeSelected
        {
            get => _isTimeSelected;
            set => OnPropertyChanged(value, ref _isTimeSelected);
        }

        public ObservableCollection<TimeViewModel> Times
        {
            get;
        } = new ObservableCollection<TimeViewModel>();

        public ObservableCollection<Axis> XAxes { get; set; }

        public ObservableCollection<Axis> YAxes { get; set; }

        public ObservableCollection<ISeries> Series { get; }

        public ICommand TemperatureCommand { get; }

        public ICommand PressureCommand { get; }

        public ICommand WindSpeedCommand { get; }

        //public ICommand WindDirectionCommand { get; }

        public ICommand HumidityCommand { get; }

        #endregion

        #region buttons
        public void Temperature(object? parameter)
        {
            Series.Clear();

            _weatherData = _weatherParserService.GetAllWeatherData(_selectedDate.Value);

            if (_weatherData != null)
            {
                for (int i = 0; i < Times.Count; ++i)
                {
                    if (Times[i].IsChecked)
                    {
                        var tempValues = new List<double>();

                        foreach (var temp in _weatherData.Values)
                        {
                            tempValues.Add(temp[i].Temperature);
                        }
                        Series.Add(new LineSeries<double> { Values = tempValues, Name = $"{Times[i].CurrentTime}.00"});
                        //Series.Add(new LineSeries<double> { Values = new List<double>() { 1, 2, 3}, Name = $"{Times[i].CurrentTime}.00"});

                    }
                }
            }
            DisableButtonsAndCheckBoxes();
        }

        public void Pressure(object? parameter)
        {
            Series.Clear();

            _weatherData = _weatherParserService.GetAllWeatherData(_selectedDate.Value);

            if (_weatherData != null)
            {
                for (int i = 0; i < Times.Count; ++i)
                {
                    if (Times[i].IsChecked)
                    {
                        var tempValues = new List<double>();

                        foreach (var temp in _weatherData.Values)
                        {
                            tempValues.Add(temp[i].Pressure);
                        }
                        Series.Add(new LineSeries<double> { Values = tempValues, Name = $"{Times[i].CurrentTime}.00" });
                        //Series.Add(new LineSeries<double> { Values = new List<double>() { 1, 2, 3}, Name = $"{Times[i].CurrentTime}.00"});

                    }
                }
            }
            DisableButtonsAndCheckBoxes();

        }

        public void WindSpeed(object? parameter)
        {
            Series.Clear();

            _weatherData = _weatherParserService.GetAllWeatherData(_selectedDate.Value);

            if (_weatherData != null)
            {
                for (int i = 0; i < Times.Count; ++i)
                {
                    if (Times[i].IsChecked)
                    {
                        var tempValues = new List<double>();

                        foreach (var temp in _weatherData.Values)
                        {
                            tempValues.Add(temp[i].WindSpeedFirst);
                        }
                        Series.Add(new LineSeries<double> { Values = tempValues, Name = $"{Times[i].CurrentTime}.00" });
                        //Series.Add(new LineSeries<double> { Values = new List<double>() { 1, 2, 3}, Name = $"{Times[i].CurrentTime}.00"});

                    }
                }
            }
            DisableButtonsAndCheckBoxes();
        }

        //public void WindDirection(object? parameter)
        //{
        //    Series.Clear();

        //    _weatherData = _weatherParserService.GetAllWeatherData(_selectedDate.Value);

        //    if (_weatherData != null)
        //    {
        //        for (int i = 0; i < Times.Count; ++i)
        //        {
        //            if (Times[i].IsChecked)
        //            {
        //                var tempValues = new List<string>();

        //                foreach (var temp in _weatherData.Values)
        //                {
        //                    tempValues.Add(temp[i].WindDirection);
        //                }
        //                Series.Add(new LineSeries<string> { Values = tempValues, Name = $"{Times[i].CurrentTime}.00" });
        //                //Series.Add(new LineSeries<double> { Values = new List<double>() { 1, 2, 3}, Name = $"{Times[i].CurrentTime}.00"});

        //            }
        //        }
        //    }
        //    DisableButtonsAndCheckBoxes();
        //}

        public void Humidity(object? parameter)
        {
            Series.Clear();

            _weatherData = _weatherParserService.GetAllWeatherData(_selectedDate.Value);

            if (_weatherData != null)
            {
                for (int i = 0; i < Times.Count; ++i)
                {
                    if (Times[i].IsChecked)
                    {
                        var tempValues = new List<double>();

                        foreach (var temp in _weatherData.Values)
                        {
                            tempValues.Add(temp[i].Humidity);
                        }
                        Series.Add(new LineSeries<double> { Values = tempValues, Name = $"{Times[i].CurrentTime}.00" });
                        //Series.Add(new LineSeries<double> { Values = new List<double>() { 1, 2, 3}, Name = $"{Times[i].CurrentTime}.00"});

                    }
                }
            }
            DisableButtonsAndCheckBoxes();
        }

        #endregion

        private void DisableButtonsAndCheckBoxes()
        {
            for (int i = 0; i < Times.Count; ++i)
            {
                Times[i].IsDateChecked = false;
                Times[i].IsChecked = false;
            }

            IsTimeSelected = false;
            SelectedDate = null;
        }

    }
}

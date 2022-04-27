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
using WeatherParser.TimerSaveDataService;
using System.Windows;
using System.Diagnostics;
using WeatherParser.GrpcService.Services;
using Google.Protobuf.WellKnownTypes;
using WeatherParser.Presentation.Entities;

namespace WeatherParser.WPF.ViewModels
{
    internal class MainWindowViewModel : NotifyPropertyChangedBase, IMainWindowViewModel
    {
        #region fields

        private DateTime? _selectedDate;

        private bool _isTimeSelected = false;

        private WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient _weatherParserService;

        private Dictionary<DateTime, List<WeatherDataPresentation>> _weatherData;

        private DateTime _firstDate;

        private DateTime _lastDate;

        #endregion

        #region ctor

        public MainWindowViewModel(WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient weatherParserService, ITimerSaveData timerSaveData)
        {
            timerSaveData.SaveData();

            Series = new ObservableCollection<ISeries>();
            XAxes = new ObservableCollection<Axis>();
            YAxes = new ObservableCollection<Axis>() { new Axis() };
            Times = new ObservableCollection<TimeViewModel>();

            TemperatureCommand = new RelayCommand(Temperature);
            PressureCommand = new RelayCommand(Pressure);
            HumidityCommand = new RelayCommand(Humidity);
            //WindDirectionCommand = new RelayCommand(WindDirection);
            WindSpeedCommand = new RelayCommand(WindSpeed);

            _weatherParserService = weatherParserService;

            RestartAppCommand = new RelayCommand(Restart);

            RefreshDataCommand = new RelayCommand(RefreshData);

            RefreshData(null);
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
                    _weatherData = GetResponseToDictionary(_weatherParserService.GetAllWeatherData(_selectedDate.Value.ToUniversalTime().ToTimestamp()));

                    for (int i = 0; i < Times.Count; ++i)
                    {
                        Times[i].IsDateChecked = true;
                    }

                    XAxes.Clear();

                    XAxes.Add(new Axis()
                    {
                        //fix
                        Labels = _weatherData.Keys.Select(s => s.Date.ToString("dd.MM.yyyy")).ToList(),
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

        public ICommand RestartAppCommand { get; }

        public ICommand RefreshDataCommand { get; }


        #endregion

        #region buttons
        public void Temperature(object? parameter)
        {
            Series.Clear();

            _weatherData = GetResponseToDictionary(_weatherParserService.GetAllWeatherData(_selectedDate.Value.ToUniversalTime().ToTimestamp()));

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

            _weatherData = GetResponseToDictionary(_weatherParserService.GetAllWeatherData(_selectedDate.Value.ToUniversalTime().ToTimestamp()));

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

            _weatherData = GetResponseToDictionary(_weatherParserService.GetAllWeatherData(_selectedDate.Value.ToUniversalTime().ToTimestamp()));

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

        //    _weatherData = GetResponseToDictionary(_weatherParserService.GetAllWeatherData(_selectedDate.Value.ToUniversalTime().ToTimestamp()));

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

            _weatherData = GetResponseToDictionary(_weatherParserService.GetAllWeatherData(_selectedDate.Value.ToUniversalTime().ToTimestamp()));

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


        private void Restart(object? parameter)
        {
            Process.Start(Process.GetCurrentProcess().MainModule.FileName);
            Application.Current.Shutdown();
        }

        private void RefreshData(object? parameter)
        {
            IsTimeSelected = false;
            SelectedDate = null;
           
            Times.Clear();

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

            FirstDate = _weatherParserService.GetFirstDate(new Empty()).ToDateTime();
            LastDate = _weatherParserService.GetLastDate(new Empty()).ToDateTime();
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

        private Dictionary<DateTime, List<WeatherDataPresentation>> GetResponseToDictionary(WeatherDataGetResponse weatherDataGetResponse)
        {
            var result = new Dictionary<DateTime, List<WeatherDataPresentation>>();

            foreach (var item in weatherDataGetResponse.WeatherDataDictionary)
            {
                var weatherDataList = new List<WeatherDataPresentation>();

                foreach(var weatherData in item.Value.WeatherDataList)
                {
                    weatherDataList.Add(new WeatherDataPresentation()
                    {
                        CollectionDate = weatherData.CollectionDate.ToDateTime(),
                        Date = weatherData.Date.ToDateTime(),
                        Temperature = weatherData.Temperature,
                        Humidity = weatherData.Humidity,
                        Pressure = weatherData.Pressure,
                        WindDirection = weatherData.WindDirection,
                        WindSpeedFirst = weatherData.WindSpeedFirst,
                        WindSpeedSecond = weatherData.WindSpeedSecond
                    });
                }

                if (!result.ContainsKey(item.Key.ToDateTime()))
                {
                    result.Add(item.Key.ToDateTime(), weatherDataList);
                }
            }
            return result;
        }
    }
}

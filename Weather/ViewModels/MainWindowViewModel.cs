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
                    var dataGetResponse = _weatherParserService.GetAllWeatherData(_selectedDate.Value.ToUniversalTime().ToTimestamp());

                    for (int i = 0; i < Times.Count; ++i)
                    {
                        Times[i].IsDateChecked = true;
                    }

                    XAxes.Clear();

                        XAxes.Add(new Axis()
                        {
                            Labels = dataGetResponse.WeatherDataDictionary.Select(s => s.Key.ToDateTime().ToString("dd.MM.yyyy")).ToList(),
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
            DisableButtonsAndCheckBoxes();
        }

        public void Pressure(object? parameter)
        {
            DisableButtonsAndCheckBoxes();

        }

        public void WindSpeed(object? parameter)
        {
            DisableButtonsAndCheckBoxes();
        }

        //public void WindDirection(object? parameter)
        //{
        
        //    DisableButtonsAndCheckBoxes();
        //}

        public void Humidity(object? parameter)
        {
            
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

    }
}

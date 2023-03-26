using Autofac;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WeatherParser.GrpcService.Services;
using WeatherParser.Presentation.Entities;
using WeatherParser.WPF.ViewModels.Contract;

namespace WeatherParser.WPF.ViewModels
{
    internal class MainWindowViewModel : NotifyPropertyChangedBase, IDataErrorInfo
    {
        #region fields

        private DateTime? _selectedDate;

        private bool _isTimeSelected = false;

        private WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient _weatherParserService;

        private DateTime? _firstDate;

        private DateTime? _lastDate;

        //resolve slow layer container like main container from mainwindow.xaml.cs
        private ILifetimeScope _container;

        private SitePresentation _selectedSite;

        private bool _haveDates = false;

        //private List<WeatherDataPresentation> _weatherDataPresentationList;

        //private int _day;

        #endregion

        #region ctor

        public MainWindowViewModel(WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient weatherParserService, ILifetimeScope container)
        {
            _weatherParserService = weatherParserService;

            MeanDeviationsViewModel = new MeanDeviationsViewModel();
            DayDeviationsViewModel = new DayDeviationsViewModel();
            ForecastViewModel = new ForecastViewModel();

            Times = new ObservableCollection<TimeViewModel>();
            Sites = new ObservableCollection<SitePresentation>();

            TemperatureCommand = new RelayCommand(Temperature);
            PressureCommand = new RelayCommand(Pressure);
            HumidityCommand = new RelayCommand(Humidity);
            WindSpeedCommand = new RelayCommand(WindSpeed);

            RestartAppCommand = new RelayCommand(Restart);

            RefreshDataCommand = new RelayCommand(RefreshData);

            RefreshData(null);
            _container = container;
        }

        #endregion

        #region props

        public DeviationsViewModel MeanDeviationsViewModel { get; }

        public DeviationsViewModel DayDeviationsViewModel { get; }

        public DeviationsViewModel ForecastViewModel { get; }


        public SitePresentation SelectedSite
        {
            get => _selectedSite;
            set
            {
                OnPropertyChanged(value, ref _selectedSite);
                if (_selectedSite != null)
                {

                    HaveDates = false;
                    SelectedDate = null;

                    DisableButtonsAndCheckBoxes();

                    var dates = _weatherParserService.GetFirstAndLastDate(new SiteID() { ID = _selectedSite.ID.ToString() });

                    FirstDate = dates.FirstDate.ToDateTime();
                    LastDate = dates.LastDate.ToDateTime();

                    HaveDates = true;
                }
            }
        }

        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set
            {
                OnPropertyChanged(value, ref _selectedDate);

                if (SelectedSite != null && SelectedDate != null)
                {
                    DisableButtonsAndCheckBoxes();

                    EnterTimes();
                }
            }

        }

        public DateTime? FirstDate
        {
            get => _firstDate;
            set => OnPropertyChanged(value, ref _firstDate);
        }

        public DateTime? LastDate
        {
            get => _lastDate;
            set => OnPropertyChanged(value, ref _lastDate);
        }

        public bool IsTimeSelected
        {
            get => _isTimeSelected;
            set => OnPropertyChanged(value, ref _isTimeSelected);
        }

        public bool HaveDates
        {
            get => _haveDates;
            set => OnPropertyChanged(value, ref _haveDates);
        }

        public ObservableCollection<TimeViewModel> Times
        {
            get;
        } = new ObservableCollection<TimeViewModel>();

        public ObservableCollection<SitePresentation> Sites
        {
            get;
        } = new ObservableCollection<SitePresentation>();

        public ICommand TemperatureCommand { get; }

        public ICommand PressureCommand { get; }

        public ICommand WindSpeedCommand { get; }

        public ICommand HumidityCommand { get; }

        public ICommand RestartAppCommand { get; }

        public ICommand RefreshDataCommand { get; }

        public ICommand DeviationsByDayCommand { get; }

        public ICommand MeanDeviationsCommand { get; }

        //public int Day
        //{
        //    get => _day;
        //    set => OnPropertyChanged(value, ref _day);
        //}

        public string Error => throw new NotImplementedException();

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;

                //switch (columnName)
                //{
                //    //TODO: error for days count!

                //    case "Day":
                //        var days = (LastDate - FirstDate).Value.Days;

                //        if ((Day < 0) || (Day > days))
                //        {
                //            error = $"Количество дней должно быть больше 0 и не меньше {days}";
                //        }
                //        break;
                //}
                return error;
            }
        }

        #endregion

        #region buttons
        public void Temperature(object? parameter)
        {
            var temperatureCommand = _container.ResolveNamed<Commands.ICommand>("TemperatureCommand");

            MeanDeviationsViewModel.ExecuteCommand(temperatureCommand, Times);
            DayDeviationsViewModel.ExecuteCommand(temperatureCommand, Times);
            ForecastViewModel.ExecuteCommand(temperatureCommand, Times);

        }

        public void Pressure(object? parameter)
        {
            var pressureCommand = _container.ResolveNamed<Commands.ICommand>("PressureCommand");

            ForecastViewModel.ExecuteCommand(pressureCommand, Times);
            MeanDeviationsViewModel.ExecuteCommand(pressureCommand, Times);
            DayDeviationsViewModel.ExecuteCommand(pressureCommand, Times);
        }


        public void WindSpeed(object? parameter)
        {
            var windSpeedCommand = _container.ResolveNamed<Commands.ICommand>("WindSpeedCommand");

            ForecastViewModel.ExecuteCommand(windSpeedCommand, Times);
            MeanDeviationsViewModel.ExecuteCommand(windSpeedCommand, Times);
            DayDeviationsViewModel.ExecuteCommand(windSpeedCommand, Times);
        }

        public void Humidity(object? parameter)
        {
            var humidityCommand = _container.ResolveNamed<Commands.ICommand>("HumidityCommand");

            ForecastViewModel.ExecuteCommand(humidityCommand, Times);
            MeanDeviationsViewModel.ExecuteCommand(humidityCommand, Times);
            DayDeviationsViewModel.ExecuteCommand(humidityCommand, Times);
        }
        #endregion

        #region private

        private void Restart(object? parameter)
        {
            Process.Start(Process.GetCurrentProcess().MainModule.FileName);
            Application.Current.Shutdown();
        }

        private void RefreshData(object? parameter)
        {
            IsTimeSelected = false;
            SelectedDate = null;
            SelectedSite = null;
            HaveDates = false;

            Times.Clear();

            Sites.Clear();

            var sitesService = _weatherParserService.GetSites(new Empty());

            foreach (var site in sitesService.Sites)
            {
                Sites.Add(new SitePresentation() { ID = new Guid(site.SiteId), Name = site.SiteName });
            }
        }

        private void DisableButtonsAndCheckBoxes()
        {
            IsTimeSelected = false;

            for (int i = 0; i < Times.Count; ++i)
            {
                Times[i].IsDateChecked = false;
                Times[i].IsChecked = false;
            }

            FirstDate = null;
            LastDate = null;
        }

        private void OnTimeChecked(object? sender, PropertyChangedEventArgs e)
        {
            IsTimeSelected = Times.Any(t => t.IsChecked);
        }

        private List<WeatherDataPresentation> CastToPresentationEntity(WeatherDataGetResponse weatherDataGetResponse)
        {
            var result = new List<WeatherDataPresentation>();

            foreach (var item in weatherDataGetResponse.WeatherData)
            {
                var weatherDataList = new List<WeatherPresentation>();

                foreach (var weatherData in item.Weather.WeatherList)
                {
                    var temps = new List<double>();
                    foreach (var temp in weatherData.Temperatures.Temperature)
                    {
                        temps.Add(temp);
                    }

                    var hums = new List<double>();
                    foreach (var hum in weatherData.Humidities.Humidity)
                    {
                        hums.Add(hum);
                    }

                    var press = new List<double>();
                    foreach (var pres in weatherData.Pressures.Pressure)
                    {
                        press.Add(pres);
                    }

                    var windSpeeds = new List<double>();
                    foreach (var windSpeed in weatherData.WindSpeeds.WindSpeed)
                    {
                        windSpeeds.Add(windSpeed);
                    }

                    var hours = new List<int>();
                    foreach (var hour in weatherData.Hours.Hour)
                    {
                        hours.Add(hour);
                    }

                    weatherDataList.Add(new WeatherPresentation()
                    {
                        Date = weatherData.Date.ToDateTime(),
                        Temperature = temps,
                        Humidity = hums,
                        Pressure = press,
                        WindSpeed = windSpeeds,
                        Hours = hours
                    });
                }

                result.Add(new WeatherDataPresentation() { TargetDate = item.TargetDate.ToDateTime(), Weather = weatherDataList });
            }
            return result;
        }

        private async Task EnterTimes()
        {
            await GetWeatherAsync();

            Times.Clear();
            IsTimeSelected = false;

            var maxTimes = ForecastViewModel.WeatherDataPresentations.Select(x => x.Weather[0].Hours.Count).Max();

            var times = ForecastViewModel.WeatherDataPresentations.FirstOrDefault(x => x.Weather[0].Hours.Count == maxTimes).Weather[0].Hours;

            for (int i = 0; i < times.Count; ++i)
            {
                Times.Add(new TimeViewModel { CurrentTime = times[i] });

                Times[i].IsChecked = false;
                Times[i].IsDateChecked = true;

                //subscribe mainViewModel on ischecked property change for change _isTimeSelected
                PropertyChangedEventManager.AddHandler(Times[i], OnTimeChecked, nameof(TimeViewModel.IsChecked));
            }
        }

        private async Task GetWeatherAsync()
        {
            ForecastViewModel.WeatherDataPresentations = CastToPresentationEntity(await _weatherParserService.GetAllWeatherDataByDayAsync(new WeatherDataRequest()
            {
                Date = DateTime.SpecifyKind((DateTime)SelectedDate, DateTimeKind.Utc).ToTimestamp(),
                SiteID = SelectedSite.ID.ToString()
            }));


            MeanDeviationsViewModel.WeatherDataPresentations = CastToPresentationEntity(await _weatherParserService.GetMeanDeviationsOfRealForecastAsync(new GetMeanDeviationsRequest()
            {
                SiteID = SelectedSite.ID.ToString(),
                Days = 3
            }));

            DayDeviationsViewModel.WeatherDataPresentations = CastToPresentationEntity(await _weatherParserService.GetDeviationsOfRealFromForecastAsync(new WeatherDataRequest()
            {
                Date = DateTime.SpecifyKind((DateTime)SelectedDate, DateTimeKind.Utc).ToTimestamp(),
                SiteID = SelectedSite.ID.ToString()
            }));
        }
        #endregion

    }
}

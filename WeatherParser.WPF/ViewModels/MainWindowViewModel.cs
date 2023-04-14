using Autofac;
using Google.Protobuf.WellKnownTypes;
using System;
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

        private bool _serverHaveRealDataOnDay;

        private bool _pressButtonReal;

        private bool _pressButtonDeviations;
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

            RealDataCommand = new RelayCommand(ShowRealDataCommand, x => _pressButtonReal);
            DeviationsDataCommand = new RelayCommand(ShowDeviationsDataCommand, x => _pressButtonDeviations);

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
            set
            {
                OnPropertyChanged(value, ref _isTimeSelected);

                if (SelectedDate != null && SelectedSite != null)
                {
                    ForecastViewModel.VisibilityProp = Visibility.Visible;
                    DayDeviationsViewModel.VisibilityProp = Visibility.Collapsed;
                }
            }
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

        public RelayCommand TemperatureCommand { get; }

        public RelayCommand PressureCommand { get; }

        public RelayCommand WindSpeedCommand { get; }

        public RelayCommand HumidityCommand { get; }

        public ICommand RestartAppCommand { get; }

        public ICommand RefreshDataCommand { get; }

        public RelayCommand DeviationsDataCommand { get; }

        public RelayCommand RealDataCommand { get; }

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
            SetPressButtonsProps();

            var temperatureCommand = _container.ResolveNamed<Commands.IWeatherCommand>("TemperatureCommand");

            MeanDeviationsViewModel.ExecuteCommand(temperatureCommand, Times);
            DayDeviationsViewModel.ExecuteCommand(temperatureCommand, Times);
            ForecastViewModel.ExecuteCommand(temperatureCommand, Times);

        }

        public void Pressure(object? parameter)
        {
            SetPressButtonsProps();

            var pressureCommand = _container.ResolveNamed<Commands.IWeatherCommand>("PressureCommand");

            ForecastViewModel.ExecuteCommand(pressureCommand, Times);
            MeanDeviationsViewModel.ExecuteCommand(pressureCommand, Times);
            DayDeviationsViewModel.ExecuteCommand(pressureCommand, Times);
        }


        public void WindSpeed(object? parameter)
        {
            SetPressButtonsProps();

            var windSpeedCommand = _container.ResolveNamed<Commands.IWeatherCommand>("WindSpeedCommand");

            ForecastViewModel.ExecuteCommand(windSpeedCommand, Times);
            MeanDeviationsViewModel.ExecuteCommand(windSpeedCommand, Times);
            DayDeviationsViewModel.ExecuteCommand(windSpeedCommand, Times);
        }

        public void Humidity(object? parameter)
        {
            SetPressButtonsProps();

            var humidityCommand = _container.ResolveNamed<Commands.IWeatherCommand>("HumidityCommand");

            ForecastViewModel.ExecuteCommand(humidityCommand, Times);
            MeanDeviationsViewModel.ExecuteCommand(humidityCommand, Times);
            DayDeviationsViewModel.ExecuteCommand(humidityCommand, Times);
        }


        private void ShowRealDataCommand(object? obj)
        {
            ForecastViewModel.VisibilityProp = Visibility.Visible;
            DayDeviationsViewModel.VisibilityProp = Visibility.Collapsed;
        }

        private void ShowDeviationsDataCommand(object? obj)
        {
            ForecastViewModel.VisibilityProp = Visibility.Collapsed;
            DayDeviationsViewModel.VisibilityProp = Visibility.Visible;
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
            _pressButtonReal = false;
            _pressButtonDeviations = false;
            RealDataCommand.NotifyCanExecuteChanged();
            DeviationsDataCommand.NotifyCanExecuteChanged();

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

            ForecastViewModel.RefreshChart();
            DayDeviationsViewModel.RefreshChart();
            MeanDeviationsViewModel.RefreshChart();
        }

        private void DisableButtonsAndCheckBoxes()
        {
            IsTimeSelected = false;

            for (int i = 0; i < Times.Count; ++i)
            {
                Times[i].IsDateChecked = false;
                Times[i].IsChecked = false;
            }

            _pressButtonReal = false;
            _pressButtonDeviations = false;
            RealDataCommand.NotifyCanExecuteChanged();
            DeviationsDataCommand.NotifyCanExecuteChanged();

            ForecastViewModel.RefreshChart();
            DayDeviationsViewModel.RefreshChart();
            MeanDeviationsViewModel.RefreshChart();
        }

        private void OnTimeChecked(object? sender, PropertyChangedEventArgs e)
        {
            IsTimeSelected = Times.Any(t => t.IsChecked);
        }


        private async Task EnterTimes()
        {
            await GetWeather();

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

        private async Task GetWeather()
        {
            await ForecastViewModel.GetWeatherAsync(_weatherParserService, SelectedSite, SelectedDate);

            _serverHaveRealDataOnDay = _weatherParserService.HaveRealDataOnDate(new WeatherDataRequest()
            {
                Date = DateTime.SpecifyKind(_selectedDate.Value.Date, DateTimeKind.Utc).ToTimestamp(),
                SiteID = _selectedSite.ID.ToString()
            }).HaveData;


            if (_serverHaveRealDataOnDay)
            {
                await DayDeviationsViewModel.GetWeatherAsync(_weatherParserService, SelectedSite, SelectedDate);
            }

            await MeanDeviationsViewModel.GetWeatherAsync(_weatherParserService, SelectedSite, SelectedDate);
        }

        private void SetPressButtonsProps()
        {
            _pressButtonReal = true;
            _pressButtonDeviations = _serverHaveRealDataOnDay && IsTimeSelected;
            RealDataCommand.NotifyCanExecuteChanged();
            DeviationsDataCommand.NotifyCanExecuteChanged();
        }
        #endregion

    }
}

using Autofac;
using Google.Protobuf.WellKnownTypes;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
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
using IContainer = Autofac.IContainer;

namespace WeatherParser.WPF.ViewModels
{
    internal class MainWindowViewModel : NotifyPropertyChangedBase
    {
        #region fields

        private DateTime? _selectedDate;

        private bool _isTimeSelected = false;

        private WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient _weatherParserService;

        private DateTime? _firstDate;

        private DateTime? _lastDate;

        private IContainer _container;

        private SitePresentation _selectedSite;

        private bool _haveDates = false;

        private Task<WeatherDataGetResponse> _weatherDataGetResponse;

        #endregion

        #region ctor

        public MainWindowViewModel(WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient weatherParserService)
        {
            _weatherParserService = weatherParserService;

            Series = new ObservableCollection<ISeries>();
            XAxes = new ObservableCollection<Axis>()
            {
                new Axis()
                {
                    LabelsPaint = new SolidColorPaint(SKColors.Black)
                    //Labeler = x => new DateTime((long)x).ToString("dd MM yyyy")
                }
            };
            YAxes = new ObservableCollection<Axis>() { new Axis() };
            Times = new ObservableCollection<TimeViewModel>();
            Sites = new ObservableCollection<SitePresentation>();

            TemperatureCommand = new RelayCommand(Temperature);
            PressureCommand = new RelayCommand(Pressure);
            HumidityCommand = new RelayCommand(Humidity);
            WindSpeedCommand = new RelayCommand(WindSpeed);

            RestartAppCommand = new RelayCommand(Restart);

            RefreshDataCommand = new RelayCommand(RefreshData);

            RefreshData(null);

            var builder = new ContainerBuilder();
            builder.RegisterModule<WPFModule>();

            _container = builder.Build();
        }

        private void OnTimeChecked(object? sender, PropertyChangedEventArgs e)
        {
            IsTimeSelected = Times.Any(t => t.IsChecked);
        }

        #endregion

        #region props

        public SitePresentation SelectedSite
        {
            get => _selectedSite;
            set
            {
                OnPropertyChanged(value, ref _selectedSite);
                if (_selectedSite != null)
                {
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

                for (int i = 0; i < Times.Count; ++i)
                {
                    Times[i].IsDateChecked = true;
                }

                if (SelectedSite != null && SelectedDate != null)
                {
                    _weatherDataGetResponse = _weatherParserService.GetAllWeatherDataByDayAsync(new WeatherDataRequest()
                    {
                        Date = DateTime.SpecifyKind((DateTime)SelectedDate, DateTimeKind.Utc).ToTimestamp(),
                        SiteID = SelectedSite.ID.ToString()
                    }).ResponseAsync;
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

        public ObservableCollection<Axis> XAxes { get; set; }

        public ObservableCollection<Axis> YAxes { get; set; }

        public ObservableCollection<ISeries> Series { get; }

        public ICommand TemperatureCommand { get; }

        public ICommand PressureCommand { get; }

        public ICommand WindSpeedCommand { get; }

        public ICommand HumidityCommand { get; }

        public ICommand RestartAppCommand { get; }

        public ICommand RefreshDataCommand { get; }


        #endregion

        #region buttons
        public void Temperature(object? parameter)
        {
            Series.Clear();
            var temperatureCommand = _container.ResolveNamed<Commands.ICommand>("TemperatureCommand");
            temperatureCommand.Execute(_weatherDataGetResponse.Result, _selectedDate, Series, _selectedSite, Times, XAxes);
        }

        public void Pressure(object? parameter)
        {
            Series.Clear();
            var pressureCommand = _container.ResolveNamed<Commands.ICommand>("PressureCommand");
            pressureCommand.Execute(_weatherDataGetResponse.Result, _selectedDate, Series, _selectedSite, Times, XAxes);
        }

        public void WindSpeed(object? parameter)
        {
            Series.Clear();
            var windSpeedCommand = _container.ResolveNamed<Commands.ICommand>("WindSpeedCommand");
            windSpeedCommand.Execute(_weatherDataGetResponse.Result, _selectedDate, Series, _selectedSite, Times, XAxes);
        }

        public void Humidity(object? parameter)
        {
            Series.Clear();
            var humidityCommand = _container.ResolveNamed<Commands.ICommand>("HumidityCommand");
            humidityCommand.Execute(_weatherDataGetResponse.Result, _selectedDate, Series, _selectedSite, Times, XAxes);
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
            SelectedSite = null;
            HaveDates = false;

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

            Series.Clear();
            Sites.Clear();

            var sitesService = _weatherParserService.GetSites(new Empty());
            foreach (var site in sitesService.Sites)
            {
                Sites.Add(new SitePresentation() { ID = new Guid(site.SiteId), Name = site.SiteName });
            }

            //if (XAxes.Any())
            //{
            //    XAxes[0].Labels = new List<string>() { string.Empty };
            //}
        }

        private void DisableButtonsAndCheckBoxes()
        {
            IsTimeSelected = false;
            HaveDates = false;
            SelectedDate = null;

            for (int i = 0; i < Times.Count; ++i)
            {
                Times[i].IsDateChecked = false;
                Times[i].IsChecked = false;
            }

            FirstDate = null;
            LastDate = null;
        }
        #endregion

    }
}

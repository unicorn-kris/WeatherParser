using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WeatherParser.GrpcService.Services;
using WeatherParser.Presentation.Entities;
using WeatherParser.WPF.Commands;

namespace WeatherParser.WPF.ViewModels.Contract
{
    internal abstract class DeviationsViewModel : NotifyPropertyChangedBase, IDeviationsViewModel
    {
        #region props

        public ObservableCollection<Axis> XAxes { get; } = new();
        public ObservableCollection<Axis> YAxes { get; } = new();

        public ObservableCollection<ISeries> Series { get; } = new();

        public List<WeatherDataPresentation> WeatherDataPresentations { get; set; }

        #endregion

        #region public

        public void ExecuteCommand(IWeatherCommand command, ObservableCollection<TimeViewModel> times)
        {
            Series.Clear();

            if (!XAxes.Any())
            {
                XAxes.Add(new Axis()
                {
                    LabelsPaint = new SolidColorPaintTask(SKColors.Black),
                    Labeler = x => new DateTime((long)x).ToString("dd/MM/yyyy"),
                    UnitWidth = TimeSpan.FromDays(1).Ticks,
                    MinStep = TimeSpan.FromDays(1).Ticks
                });
            }

            if (!YAxes.Any())
            {
                YAxes.Add(new Axis());
            }

            command.Execute(WeatherDataPresentations, Series, times);
        }

        public abstract Task GetWeatherAsync(WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient weatherDataProtoGismeteo,
            SitePresentation selectedSite,
            DateTime? selectedDate);


        #endregion

        protected List<WeatherDataPresentation> CastToPresentationEntity(WeatherDataGetResponse weatherDataGetResponse)
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
    }
}

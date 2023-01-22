using Helpers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WeatherParser.Repository.Contract;
using WeatherParser.Repository.Entities;
using WeatherParser.Service.Contract;
using WeatherParser.Service.Entities;
using WeatherParser.Service.GismeteoService.Contract;

namespace WeatherParser.Service
{
    public class Service : IService
    {
        private readonly IWeatherParserRepository _weatherParserRepository;
        private readonly IWeatherParserServiceGismeteo _weatherParserServiceGismeteo;
        private PeriodicTimer _timer;

        public Service(IWeatherParserRepository weatherParserRepository,
            IWeatherParserServiceGismeteo weatherParserServiceGismeteo)
        {
            _weatherParserRepository = weatherParserRepository;
            _weatherParserServiceGismeteo = weatherParserServiceGismeteo;

            _timer = new PeriodicTimer(TimeSpan.FromDays(1));
            Task timerTask = HandleTimerAsync(_timer);
        }

        private async Task HandleTimerAsync(PeriodicTimer timer)
        {
            try
            {
                await SaveWeatherDataAsync().ConfigureAwait(false);

                while (await timer.WaitForNextTickAsync())
                {
                    await SaveWeatherDataAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Save weather exception " + ex.Message);
            }

        }

        public async Task<List<WeatherDataService>> GetAllWeatherDataByDayAsync(DateTime targetDate, Guid siteId)
        {
            var data = await _weatherParserRepository.GetAllWeatherDataByDayAsync(targetDate, siteId).ConfigureAwait(false);

            //map weatherdatarepository to weatherdataservice
            var weatherDataList = new List<WeatherDataService>();

            foreach (var weatherData in data)
            {
                var weathers = new List<WeatherService>();

                foreach (var weather in weatherData.Weather)
                {
                    weathers.Add(new WeatherService()
                    {
                        Date = weather.Date,
                        Humidity = weather.Humidity,
                        Pressure = weather.Pressure,
                        Temperature = weather.Temperature,
                        WindDirection = weather.WindDirection,
                        WindSpeed = weather.WindSpeed
                    });
                }
                weatherDataList.Add(new WeatherDataService()
                {
                    SiteId = weatherData.SiteID,
                    TargetDate = weatherData.TargetDate,
                    Weather = weathers
                });
            }
            return weatherDataList;
        }

        public async Task<(DateTime firstDate, DateTime lastDate)> GetFirstAndLastDateAsync(Guid siteId)
        {
            return await _weatherParserRepository.GetFirstAndLastDateAsync(siteId).ConfigureAwait(false);
        }

        public async Task<List<SiteService>> GetSitesAsync()
        {
            var repositorySites = await _weatherParserRepository.GetSitesAsync().ConfigureAwait(false);

            //map siterepository to siteservice
            var sites = new List<SiteService>();

            foreach (var site in repositorySites)
            {
                sites.Add(new SiteService()
                {
                    ID = site.ID,
                    Name = site.Name,
                    Rating = site.Rating
                });
            }
            return sites;
        }

        public async Task SaveWeatherDataAsync()
        {
            await SitesHelper.SaveSites().ConfigureAwait(false);

            var weatherData = _weatherParserServiceGismeteo.SaveWeatherData();

            //convert weatherService to weatherRepository
            var weatherRepositoryList = new WeatherDataRepository()
            {
                TargetDate = weatherData.TargetDate,
                Weather = new List<WeatherRepository>(),
                SiteID = weatherData.SiteId
            };

            foreach (var weather in weatherData.Weather)
            {
                weatherRepositoryList.Weather.Add(new WeatherRepository()
                {
                    Date = weather.Date,
                    Pressure = weather.Pressure,
                    Humidity = weather.Humidity,
                    Temperature = weather.Temperature,
                    WindDirection = weather.WindDirection,
                    WindSpeed = weather.WindSpeed
                });
            }

            await _weatherParserRepository.SaveWeatherDataAsync(weatherRepositoryList).ConfigureAwait(false);
        }
    }
}

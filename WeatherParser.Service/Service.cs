using Helpers;
using System;
using System.Collections.Generic;
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

        public Service(IWeatherParserRepository weatherParserRepository,
            IWeatherParserServiceGismeteo weatherParserServiceGismeteo)
        {
            _weatherParserRepository = weatherParserRepository;
            _weatherParserServiceGismeteo = weatherParserServiceGismeteo;
        }

        public async Task<List<WeatherDataService>> GetAllWeatherDataAsync(DateTime targetDate, Guid siteId)
        {
            var data = await _weatherParserRepository.GetAllWeatherDataAsync(targetDate, siteId);

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
            return await _weatherParserRepository.GetFirstAndLastDateAsync(siteId);
        }

        public async Task<List<SiteService>> GetSitesAsync()
        {
            var repositorySites = await _weatherParserRepository.GetSitesAsync();

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
            await SitesHelper.SaveSites();

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

            await _weatherParserRepository.SaveWeatherDataAsync(weatherRepositoryList);
        }
    }
}

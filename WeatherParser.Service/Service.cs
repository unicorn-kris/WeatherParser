using System;
using System.Collections.Generic;
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

        public List<WeatherDataService> GetAllWeatherData(DateTime targetDate, Guid siteId)
        {
            var data = _weatherParserRepository.GetAllWeatherData(targetDate, siteId);

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

        public (DateTime firstDate, DateTime lastDate) GetFirstAndLastDate(Guid siteId)
        {
            return _weatherParserRepository.GetFirstAndLastDate(siteId);
        }

        public List<SiteService> GetSites()
        {
            var repositorySites = _weatherParserRepository.GetSites();

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

        public void SaveWeatherData()
        {
            List<List<WeatherDataService>> weatherToSave = new List<List<WeatherDataService>>();

            //add weatherData from plugins
            weatherToSave.Add(_weatherParserServiceGismeteo.SaveWeatherData());

            foreach (var weatherDataFromSite in weatherToSave)
            {
                foreach (var weatherData in weatherDataFromSite)
                {

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

                    _weatherParserRepository.SaveWeatherData(weatherRepositoryList);
                }
            }
        }
    }
}

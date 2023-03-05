using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherParser.Repository.Contract;
using WeatherParser.Repository.Entities;
using WeatherParser.Service.Common;
using WeatherParser.Service.Contract;
using WeatherParser.Service.Entities;

namespace WeatherParser.Service
{
    public class Service : IService
    {
        private readonly IWeatherParserRepository _weatherParserRepository;
        private IEnumerable<IWeatherPlugin> _plugins;

        public Service(IWeatherParserRepository weatherParserRepository,
            IEnumerable<IWeatherPlugin> plugins)
        {
            _weatherParserRepository = weatherParserRepository;
            _plugins = plugins;
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

        //deviations of real from forecast weather data
        public async Task<List<WeatherDataService>> GetDeviationsOfRealFromForecast(DateTime targetDate, Guid siteId)
        {
            var data = await _weatherParserRepository.GetAllWeatherDataByDayAsync(targetDate, siteId).ConfigureAwait(false);

            if (data.Any(x => x.TargetDate.Date.Equals(targetDate.Date)))
            {
                return GetDeviations(data, targetDate);
            }
            else
            {
                throw new Exception($"Have no real weather on {targetDate.Date.ToShortDateString()}");
            }
        }

        public async Task<List<WeatherDataService>> GetDeviationsOfRealForecast(Guid siteId, int days)
        {
            var data1 = await _weatherParserRepository.GetAllWeatherDataByDayAsync(DateTime.Now, siteId).ConfigureAwait(false);
            //map weatherdatarepository to weatherdataservice
            var weatherDataList = new List<WeatherDataService>();
            var countOfDates = 0;

            var weathers = new List<WeatherService>();
            var dates = new List<DateTime>();

            foreach (var dateInGraph in data1.Select(x => x.TargetDate.Date))
            {
               if ( data1.Where(x => x.Weather.Where(y => y.Date.Date.Equals(dateInGraph)).Count() == days).Count() > 0)
                {
                    dates.Add(dateInGraph);
                }
            }

            foreach (var date in dates)
            {
                var data = await _weatherParserRepository.GetAllWeatherDataByDayAsync(date, siteId).ConfigureAwait(false);

                ++countOfDates;

                weatherDataList = GetDeviations(data, date);

                foreach (var weatherData in weatherDataList)
                {
                    if (!weathers.Any())
                    {
                        foreach (var weather in weatherData.Weather)
                        {
                            weathers.Add(weather);
                        }
                    }
                    else
                    {
                        for (var weather in weatherData.Weather)
                        {
                            weathers.Add(weather);
                        }
                    }
                }

            }
            weatherDataList.Add(new WeatherDataService()
            {
                SiteId = siteId,
                TargetDate = new DateTime(0, 0, countOfDates),
                Weather = weathers
            });
        }

        private List<WeatherDataService> GetDeviations(List<WeatherDataRepository> data, DateTime targetDate)
        {
            //map weatherdatarepository to weatherdataservice
            var weatherDataList = new List<WeatherDataService>();

            foreach (var weatherData in data)
            {
                var weathers = new List<WeatherService>();
                var targetData = weatherData.Weather.Where(x => x.Date.Date.Equals(targetDate.Date)).FirstOrDefault();

                foreach (var weather in weatherData.Weather)
                {
                    var humidities = new List<int>();
                    var temperatures = new List<double>();
                    var pressures = new List<int>();
                    var windSpeeds = new List<double>();

                    //all arrays of weather have a one size for one site
                    for (int i = 0; i < targetData.Temperature.Count; i++)
                    {
                        temperatures.Add(targetData.Temperature[i] - weather.Temperature[i]);
                        humidities.Add(targetData.Humidity[i] - weather.Humidity[i]);
                        pressures.Add(targetData.Pressure[i] - weather.Pressure[i]);
                        windSpeeds.Add(targetData.WindSpeed[i] - weather.WindSpeed[i]);
                    }

                    weathers.Add(new WeatherService()
                    {
                        Date = weather.Date,
                        Humidity = humidities,
                        Pressure = pressures,
                        Temperature = temperatures,
                        WindSpeed = windSpeeds
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

    }
}

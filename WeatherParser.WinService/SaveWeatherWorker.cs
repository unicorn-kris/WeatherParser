﻿using Autofac.Features.AttributeFilters;
using Autofac.Features.Indexed;
using Sgbj.Cron;
using WeatherParser.Repository.Contract;
using WeatherParser.Repository.Entities;
using WeatherParser.Service.Common;

namespace WeatherParser.WinService
{
    public class SaveWeatherWorker : BackgroundService
    {
        private IWeatherParserRepository _weatherParserRepository;
        private CronTimer _timer;
        IEnumerable<IWeatherPlugin> _plugins;

        public SaveWeatherWorker(IWeatherParserRepository weatherParserRepository,
            [KeyFilter("Gismeteo")] IWeatherPlugin weatherParserServiceGismeteo,
            [KeyFilter("Foreca")] IWeatherPlugin weatherParserServiceForeca,
            IIndex<string, IWeatherPlugin> index,
            IEnumerable<IWeatherPlugin> plugins)
        {
            _weatherParserRepository = weatherParserRepository;

            _plugins = plugins;

            _timer = new CronTimer("1 * * * *", TimeZoneInfo.Local);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (await _timer.WaitForNextTickAsync())
            {
                await SaveWeatherDataAsync();
            }
        }

        private async Task SaveWeatherDataAsync()
        {
            foreach (var weatherPugin in _plugins)
            {
                var weatherData = await weatherPugin.SaveWeatherDataAsync();

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
}
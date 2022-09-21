using System;
using System.Collections.Generic;
using WeatherParser.Repository.Entities;

namespace WeatherParser.Repository.Contract
{
    public interface IWeatherParserRepository
    {
        void SaveWeatherData(WeatherDataRepository weatherData);
        List<WeatherDataRepository> GetAllWeatherData(DateTime targetDate, Guid siteId);
        DateTime GetFirstAndLastDate(Guid siteId);
        List<SiteRepository> GetSites();
    }
}

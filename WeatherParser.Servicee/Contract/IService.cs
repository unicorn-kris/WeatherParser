using System;
using System.Collections.Generic;
using WeatherParser.Service.Entities;

namespace WeatherParser.Service.Contract
{
    public interface IService
    {
        void SaveWeatherData();
        List<WeatherDataService> GetAllWeatherData(DateTime targetDate, Guid siteId);
        List<DateTime> GetFirstAndLastDate(Guid siteId);
        List<SiteService> GetSites();
    }
}

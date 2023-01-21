using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeatherParser.Service.Entities;

namespace WeatherParser.Service.Contract
{
    public interface IService
    {
        Task SaveWeatherDataAsync();
        Task<List<WeatherDataService>> GetAllWeatherDataByDayAsync(DateTime targetDate, Guid siteId);
        Task<(DateTime firstDate, DateTime lastDate)> GetFirstAndLastDateAsync(Guid siteId);
        Task<List<SiteService>> GetSitesAsync();
    }
}

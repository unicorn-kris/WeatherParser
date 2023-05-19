using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeatherParser.Service.Entities;

namespace WeatherParser.Service.Contract
{
    public interface IService
    {
        Task<List<WeatherDataService>> GetAllWeatherDataByDayAsync(DateTime targetDate, Guid siteId);

        Task<(DateTime firstDate, DateTime lastDate)> GetFirstAndLastDateAsync(Guid siteId);

        Task<List<SiteService>> GetSitesAsync();

        Task<List<WeatherDataService>> GetDeviationsOfRealFromForecast(DateTime targetDate, Guid siteId);

        Task<List<WeatherDataService>> GetMeanDeviationsOfRealForecast(Guid siteId, int days);

        Task<bool> HaveRealDataOnDay(DateTime targetDate, Guid siteId);

        Task SaveDataInExcel(string path, Guid siteId);
    }
}

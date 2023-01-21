using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeatherParser.Repository.Entities;

namespace WeatherParser.Repository.Contract
{
    public interface IWeatherParserRepository
    {
        Task SaveWeatherDataAsync(WeatherDataRepository weatherData);

        Task<List<WeatherDataRepository>> GetAllWeatherDataByDayAsync(DateTime targetDate, Guid siteId);

        Task<(DateTime, DateTime)> GetFirstAndLastDateAsync(Guid siteId);

        Task<List<SiteRepository>> GetSitesAsync();
    }
}

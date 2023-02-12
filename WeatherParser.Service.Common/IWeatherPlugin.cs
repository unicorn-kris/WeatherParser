using WeatherParser.Service.Entities;

namespace WeatherParser.Service.Common
{
    public interface IWeatherPlugin
    {
        Guid SiteID { get; }
        string Name { get; }
        Task<WeatherDataService> SaveWeatherDataAsync();
    }
}
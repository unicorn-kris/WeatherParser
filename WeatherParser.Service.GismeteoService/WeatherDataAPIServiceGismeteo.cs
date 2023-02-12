using WeatherParser.Service.Common;
using WeatherParser.Service.Entities;

namespace WeatherParser.Service.Plugins.GismeteoService
{
    public class WeatherDataAPIServiceGismeteo : IWeatherPlugin
    {
        public Guid SiteID => new Guid("ed13908a-c2dc-4edb-bb9c-1678300a3435");
        public string Name => "Gismeteo";
        public Task<WeatherDataService> SaveWeatherDataAsync()
        {
            throw new NotImplementedException();
        }
    }
}

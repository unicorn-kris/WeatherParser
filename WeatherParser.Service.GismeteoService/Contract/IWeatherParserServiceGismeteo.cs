using WeatherParser.Service.Entities;

namespace WeatherParser.Service.GismeteoService.Contract
{
    public interface IWeatherParserServiceGismeteo
    {
        WeatherDataService SaveWeatherData();
    }
}

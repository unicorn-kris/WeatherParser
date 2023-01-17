using WeatherParser.Service.Entities;

namespace WeatherParser.Service.GismeteoService.Contract
{
    public interface IWeatherParserServiceForeca
    {
        List<WeatherDataService> SaveWeatherData();
    }
}

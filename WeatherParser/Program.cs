using WeatherParser.Dependencies;
using WeatherParser.Entities;
using WeatherParser.ServiceContracts;

namespace WeatherParser.ConsolePL
{
    class Program
    {
        static void Main(string[] args)
        {
            IWeatherParserService weatherParserService = DependencyResolver.WeatherParserService;
            var result = weatherParserService.GetDataAsync(MainUrlSaratov.urlWeatherToday);

        }
    }
}

using WeatherParser.RepositoryContracts;
using WeatherParser.RepositoryFiles;
using WeatherParser.Service;
using WeatherParser.ServiceContracts;

namespace WeatherParser.Dependencies
{
    public class DependencyResolver
    {
        public static IWeatherParserRepository WeatherParserRepository => new WeatherDataRepository();
        public static IWeatherParserService WeatherParserService => new WeatherDataService(WeatherParserRepository);
    }
}

using System.Collections.Generic;
using WeatherParser.Entities;

namespace WeatherParser.RepositoryContracts
{
    public interface IWeatherParserRepository
    {
        bool SaveWeatherData(List<WeatherData> listOfWeatherData);
    }
}

using System;
using System.Collections.Generic;
using WeatherParser.Presentation.Entities;

namespace WeatherParser.RepositoryContracts
{
    public interface IWeatherParserRepository
    {
        bool SaveWeatherData(List<WeatherData> listOfWeatherData);
        Dictionary<DateTime, List<WeatherData>> GetWeatherData(DateTime targetDate);

    }
}

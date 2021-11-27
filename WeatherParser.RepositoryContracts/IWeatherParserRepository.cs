using System;
using System.Collections.Generic;
using WeatherParser.Entities;

namespace WeatherParser.RepositoryContracts
{
    public interface IWeatherParserRepository
    {
        bool SaveWeatherData(List<WeatherData> listOfWeatherData);
        Dictionary<DateTime, List<List<WeatherData>>> GetWeatherData(DateTime targetDate);

    }
}

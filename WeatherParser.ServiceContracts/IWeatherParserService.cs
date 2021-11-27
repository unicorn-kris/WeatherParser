using System;
using System.Collections.Generic;
using WeatherParser.Entities;

namespace WeatherParser.ServiceContracts
{
    public interface IWeatherParserService
    {
        bool SaveWeatherData(string url);
        Dictionary<DateTime, List<List<WeatherData>>> GetWeatherData(DateTime targetDate);

    }
}

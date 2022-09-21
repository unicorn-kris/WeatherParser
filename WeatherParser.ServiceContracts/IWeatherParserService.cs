using System;
using System.Collections.Generic;
using WeatherParser.Presentation.Entities;

namespace WeatherParser.ServiceContracts
{
    public interface IWeatherParserService
    {
        bool SaveWeatherData(string url);
        Dictionary<DateTime, List<WeatherData>> GetWeatherData(DateTime targetDate);

    }
}

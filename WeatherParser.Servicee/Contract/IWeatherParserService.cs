using System;
using System.Collections.Generic;
using WeatherParser.Entities;

namespace WeatherParser.Contract
{
    public interface IWeatherParserService
    {
        bool SaveWeatherData(string url);

        Dictionary<DateTime, List<WeatherData>> GetAllWeatherData(DateTime targetDate);

        DateTime GetFirstDate();

        DateTime GetLastDate();
    }
}

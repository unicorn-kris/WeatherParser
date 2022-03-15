using System;
using System.Collections.Generic;
using WeatherParser.Entities;

namespace WeatherParser.Service.Contract
{
    public interface IWeatherParserService
    {
        void SaveWeatherData(string url, int dayNum);

        Dictionary<DateTime, List<WeatherData>> GetAllWeatherData(DateTime targetDate);

        DateTime GetFirstDate();

        DateTime GetLastDate();
    }
}

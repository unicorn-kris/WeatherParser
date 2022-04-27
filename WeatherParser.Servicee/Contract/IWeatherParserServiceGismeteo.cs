using System;
using System.Collections.Generic;
using WeatherParser.Service.Entities;

namespace WeatherParser.Service.Contract
{
    public interface IWeatherParserServiceGismeteo
    {
        void SaveWeatherData(string url, int dayNum);

        Dictionary<DateTime, List<WeatherDataService>> GetAllWeatherData(DateTime targetDate);

        DateTime GetFirstDate();

        DateTime GetLastDate();
    }
}

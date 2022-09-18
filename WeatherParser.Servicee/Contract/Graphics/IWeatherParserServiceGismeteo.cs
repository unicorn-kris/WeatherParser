using System;
using System.Collections.Generic;
using WeatherParser.Service.Entities;

namespace WeatherParser.Servicee.Contract.Graphics
{
    public interface IWeatherParserServiceGismeteo
    {
        void SaveWeatherData(string url, int dayNum);

        List<WeatherDataService> GetAllWeatherData(DateTime targetDate);

        DateTime GetFirstDate();

        DateTime GetLastDate();
    }
}

using System;
using System.Collections.Generic;
using WeatherParser.Entities;

namespace WeatherParser.Repository.Contract
{
    public interface IWeatherParserRepository
    {
        void SaveWeatherData(List<WeatherData> listOfWeatherData);

        Dictionary<DateTime, List<WeatherData>> GetAllWeatherData(DateTime targetDate);

        DateTime GetFirstDate();

        DateTime GetLastDate();
    }
}

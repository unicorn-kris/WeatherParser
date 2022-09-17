using System;
using System.Collections.Generic;
using WeatherParser.Repository.Entities;

namespace WeatherParser.Repository.Contract
{
    public interface IWeatherParserRepository
    {
        void SaveWeatherData(DateTime targetDate, WeatherRepository listOfWeatherData);

        Dictionary<DateTime, List<WeatherRepository>> GetAllWeatherData(DateTime targetDate);

        DateTime GetFirstDate();

        DateTime GetLastDate();
    }
}

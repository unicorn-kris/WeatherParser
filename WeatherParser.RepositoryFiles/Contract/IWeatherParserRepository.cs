using System;
using System.Collections.Generic;
using WeatherParser.Repository.Entities;

namespace WeatherParser.Repository.Contract
{
    public interface IWeatherParserRepository
    {
        void SaveWeatherData(List<WeatherDataRepository> listOfWeatherData);

        Dictionary<DateTime, List<WeatherDataRepository>> GetAllWeatherData(DateTime targetDate);

        DateTime GetFirstDate();

        DateTime GetLastDate();
    }
}

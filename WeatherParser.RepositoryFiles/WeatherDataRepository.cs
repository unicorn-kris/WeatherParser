using System;
using System.Collections.Generic;
using WeatherParser.Entities;
using WeatherParser.RepositoryContracts;

namespace WeatherParser.RepositoryFiles
{
    public class WeatherDataRepository : IWeatherParserRepository
    {
        public bool SaveWeatherData(List<WeatherData> listOfWeatherData)
        {
            throw new NotImplementedException();
        }
    }
}

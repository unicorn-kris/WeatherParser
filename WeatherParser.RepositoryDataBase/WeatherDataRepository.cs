using System;
using System.Collections.Generic;
using WeatherParser.Entities;
using WeatherParser.RepositoryContracts;

namespace WeatherParser.RepositoryDataBase
{
    public class WeatherDataRepository : IWeatherParserRepository
    {
        public Dictionary<DateTime, List<List<WeatherData>>> GetWeatherData(DateTime targetDate)
        {

            Dictionary<DateTime, List<List<WeatherData>>> dataInFiles = new Dictionary<DateTime, List<List<WeatherData>>>();
            //когда был составлен прогноз + список, где каждый список это weatherData на 8 часов
            return null;
        }

        public bool SaveWeatherData(List<WeatherData> listOfWeatherData)
        {
            throw new NotImplementedException();
        }
    }
}

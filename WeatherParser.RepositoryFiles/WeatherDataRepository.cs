using System;
using WeatherParser.RepositoryContracts;

namespace WeatherParser.RepositoryFiles
{
    public class WeatherDataRepository : IWeatherParserRepository
    {
        public bool GetData(string url)
        {
            throw new NotImplementedException();
        }

        public bool GetHumidity(string url)
        {
            throw new NotImplementedException();
        }

        public bool GetPressure(string url)
        {
            throw new NotImplementedException();
        }

        public bool GetTemperature(string url)
        {
            throw new NotImplementedException();
        }

        public bool GetWindDirection(string url)
        {
            throw new NotImplementedException();
        }

        public bool GetWindSpeed(string url)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherParser.RepositoryContracts
{
    public interface IWeatherParserRepository
    {
        bool GetData(string url);
        bool GetTemperature(string url);
        bool GetPressure(string url);
        bool GetWindSpeed(string url);
        bool GetWindDirection(string url);
        bool GetHumidity(string url);
    }
}

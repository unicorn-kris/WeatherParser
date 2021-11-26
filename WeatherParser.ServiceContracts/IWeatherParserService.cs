using System.Threading.Tasks;

namespace WeatherParser.ServiceContracts
{
    public interface IWeatherParserService
    {
        bool GetDataAsync(string url);
    }
}

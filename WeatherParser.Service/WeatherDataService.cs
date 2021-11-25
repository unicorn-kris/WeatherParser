using AngleSharp;
using System;
using System.Configuration;
using System.Runtime.Remoting.Contexts;
using System.Security.Policy;
using System.Threading.Tasks;
using WeatherParser.RepositoryContracts;
using WeatherParser.ServiceContracts;

namespace WeatherParser.Service
{
    public class WeatherDataService : IWeatherParserService
    {
        private readonly IWeatherParserRepository _weatherParserRepository;

        private IBrowsingContext context = BrowsingContext.New(AngleSharp.Configuration.Default);
        public WeatherDataService(IWeatherParserRepository weatherParserRepository)
        {
            _weatherParserRepository = weatherParserRepository;
        }
        public async Task<bool> GetDataAsinc(string url)
        {
            var document = await context.OpenAsync(url);


        }

        public string GetHumidity(string url)
        {
            throw new NotImplementedException();
        }

        public string GetPressure(string url)
        {
            throw new NotImplementedException();
        }

        public string GetTemperature(string url)
        {
            throw new NotImplementedException();
        }

        public string GetWindDirection(string url)
        {
            throw new NotImplementedException();
        }

        public string GetWindSpeed(string url)
        {
            throw new NotImplementedException();
        }
    }
}

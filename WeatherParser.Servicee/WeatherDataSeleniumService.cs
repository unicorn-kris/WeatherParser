using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherParser.Entities;
using WeatherParser.Repository.Contract;
using WeatherParser.Service.Contract;

namespace WeatherParser.Service
{
    class WeatherDataSeleniumService: IWeatherParserService
    {
        private readonly IWeatherParserRepository _weatherParserRepository;

        public WeatherDataSeleniumService(IWeatherParserRepository weatherParserRepository)
        {
            _weatherParserRepository = weatherParserRepository;
        }

        public Dictionary<DateTime, List<WeatherData>> GetAllWeatherData(DateTime targetDate)
        {
            return _weatherParserRepository.GetAllWeatherData(targetDate);
        }

        public DateTime GetFirstDate()
        {
            return _weatherParserRepository.GetFirstDate();
        }

        public DateTime GetLastDate()
        {
            return _weatherParserRepository.GetLastDate();
        }

        public void SaveWeatherData(string url, int dayNum)
        {
            List<WeatherData> listOfWeatherData = new List<WeatherData>(8);



        }

    }
}

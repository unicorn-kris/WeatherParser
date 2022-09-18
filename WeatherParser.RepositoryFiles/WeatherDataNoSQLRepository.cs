using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherParser.Repository.Contract;
using WeatherParser.Repository.Entities;

namespace WeatherParser.RepositoryFiles
{
    public class WeatherDataNoSQLRepository : IWeatherParserRepository
    {
        public DateTime GetFirstDate()
        {
            throw new NotImplementedException();
        }

        public DateTime GetLastDate()
        {
            throw new NotImplementedException();
        }

        public void SaveWeatherData(WeatherDataRepository weatherData)
        {
            throw new NotImplementedException();
        }

        List<WeatherDataRepository> IWeatherParserRepository.GetAllWeatherData(DateTime targetDate)
        {
            throw new NotImplementedException();
        }
    }
}

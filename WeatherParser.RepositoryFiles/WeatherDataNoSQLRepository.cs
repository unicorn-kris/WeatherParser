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
        public Dictionary<DateTime, List<WeatherRepository>> GetAllWeatherData(DateTime targetDate)
        {
            throw new NotImplementedException();
        }

        public DateTime GetFirstDate()
        {
            throw new NotImplementedException();
        }

        public DateTime GetLastDate()
        {
            throw new NotImplementedException();
        }

        public void SaveWeatherData(DateTime targetDate, WeatherRepository listOfWeatherData)
        {
            throw new NotImplementedException();
        }
    }
}

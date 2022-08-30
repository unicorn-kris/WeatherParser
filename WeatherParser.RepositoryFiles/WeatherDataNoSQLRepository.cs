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
        public Dictionary<DateTime, List<WeatherDataRepository>> GetAllWeatherData(DateTime targetDate)
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

        public void SaveWeatherData(List<WeatherDataRepository> listOfWeatherData)
        {
            throw new NotImplementedException();
        }
    }
}

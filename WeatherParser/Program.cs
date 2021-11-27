using System;
using WeatherParser.Dependencies;
using WeatherParser.Entities;
using WeatherParser.ServiceContracts;

namespace WeatherParser.ConsolePL
{
    class Program
    {
        static void Main(string[] args)
        {
            IWeatherParserService weatherParserService = DependencyResolver.WeatherParserService;
            try
            {
                var resultToday= weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeatherToday);
                var resultTomorrow = weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeatherTomorrow); 
                var result3Day = weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeather3Day);
                var result4Day = weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeater4Day);
                var result5Day = weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeater5Day);
                var result6Day = weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeater6Day);
                var result7Day = weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeater7Day);
            }
            catch
            {
                Console.WriteLine("Error");
            }

           var result = weatherParserService.GetWeatherData(DateTime.Now);
        }
    }
}

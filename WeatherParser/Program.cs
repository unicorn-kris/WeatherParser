using Autofac;
using System;
using WeatherParser.Contract;
using WeatherParser.Entities;
using WeatherParser.Repository.Contract;

namespace WeatherParser.ConsolePL
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<WeatherParserConsoleModule>();

            var container = builder.Build();

            IWeatherParserService weatherParserService = container.Resolve<IWeatherParserService>();

            try
            {
                var resultToday = weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeatherToday);
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

using Autofac;
using System;
using WeatherParser.Service.Contract;
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
                var resultToday = weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeatherToday, 0);
                var resultTomorrow = weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeatherTomorrow, 1);
                var result3Day = weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeather3Day, 2);
                var result4Day = weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeater4Day, 3);
                var result5Day = weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeater5Day, 4);
                var result6Day = weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeater6Day, 5);
                var result7Day = weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeater7Day, 6);
            }
            catch
            {
                Console.WriteLine("Error");
            }

            var result = weatherParserService.GetAllWeatherData(DateTime.Now);

            Console.ReadLine();
        }
    }
}

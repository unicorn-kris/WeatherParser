using Autofac;
using System;
using WeatherParser.Entities;
using WeatherParser.Service.Contract;

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
                weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeatherToday, 0);
                weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeatherTomorrow, 1);
                weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeather3Day, 2);
                weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeater4Day, 3);
                weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeater5Day, 4);
                weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeater6Day, 5);
                weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeater7Day, 6);
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

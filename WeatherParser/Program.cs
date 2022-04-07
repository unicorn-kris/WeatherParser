using Autofac;
using System;
using WeatherParser.Entities.Urls;
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
                weatherParserService.SaveWeatherData(UrlsSaratovGismeteo.urlWeatherToday, 0);
                weatherParserService.SaveWeatherData(UrlsSaratovGismeteo.urlWeatherTomorrow, 1);
                weatherParserService.SaveWeatherData(UrlsSaratovGismeteo.urlWeather3Day, 2);
                weatherParserService.SaveWeatherData(UrlsSaratovGismeteo.urlWeater4Day, 3);
                weatherParserService.SaveWeatherData(UrlsSaratovGismeteo.urlWeater5Day, 4);
                weatherParserService.SaveWeatherData(UrlsSaratovGismeteo.urlWeater6Day, 5);
                weatherParserService.SaveWeatherData(UrlsSaratovGismeteo.urlWeater7Day, 6);
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

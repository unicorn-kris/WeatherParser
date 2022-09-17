using Autofac;
using Google.Protobuf.WellKnownTypes;
using System;
using WeatherParser.GrpcService.Services;
using WeatherParser.Presentation.Entities.Urls;

namespace WeatherParser.ConsolePL
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<WeatherParserConsoleModule>();

            var container = builder.Build();

            WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient weatherParserService = container.Resolve<WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient>();

            try
            {
                weatherParserService.SaveWeatherData(new WeatherDataSaveRequest() { Url = UrlsSaratovGismeteo.urlWeatherToday, Day = 0 });
                weatherParserService.SaveWeatherData(new WeatherDataSaveRequest() { Url = UrlsSaratovGismeteo.urlWeatherTomorrow, Day = 1 });
                weatherParserService.SaveWeatherData(new WeatherDataSaveRequest() { Url = UrlsSaratovGismeteo.urlWeather3Day, Day = 2 });
                weatherParserService.SaveWeatherData(new WeatherDataSaveRequest() { Url = UrlsSaratovGismeteo.urlWeater4Day, Day = 3 });
                weatherParserService.SaveWeatherData(new WeatherDataSaveRequest() { Url = UrlsSaratovGismeteo.urlWeater5Day, Day = 4 });
                weatherParserService.SaveWeatherData(new WeatherDataSaveRequest() { Url = UrlsSaratovGismeteo.urlWeater6Day, Day = 5 });
                weatherParserService.SaveWeatherData(new WeatherDataSaveRequest() { Url = UrlsSaratovGismeteo.urlWeater7Day, Day = 6 });
            }
            catch
            {
                Console.WriteLine("Error");
            }

            var result = weatherParserService.GetAllWeatherData(DateTime.UtcNow.ToTimestamp());

            Console.ReadLine();
        }
    }
}

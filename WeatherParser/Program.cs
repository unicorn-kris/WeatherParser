using Autofac;
using Google.Protobuf.WellKnownTypes;
using System;
using WeatherParser.GrpcService.Services;

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
                weatherParserService.SaveWeatherData(new Empty());
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

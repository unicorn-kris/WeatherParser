using Autofac;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Serilog;
using System;
using System.Net.Http;
using WeatherParser.GrpcService.Services;
using WeatherParser.WPF.Commands;
using WeatherParser.WPF.Decorators;
using WeatherParser.WPF.ViewModels;
using WeatherParser.WPF.ViewModels.Contract;

namespace WeatherParser.WPF
{
    public class WPFModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => GrpcChannel.ForAddress(
                "http://localhost:5004", new GrpcChannelOptions
                {
                    HttpHandler = new GrpcWebHandler( new HttpClientHandler()) { HttpVersion = new Version(1, 1) }
                }))
                .As<ChannelBase>().SingleInstance();

            builder.RegisterType<WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient>();

            builder.RegisterType<MainWindowViewModel>().AsSelf();
            //builder.RegisterType<DeviationsViewModel>().As<IDeviationsViewModel>();


            builder.Register<ILogger>(log =>
            {
                return new LoggerConfiguration()
                    .WriteTo.File("log.txt")
                    .CreateLogger();
            }).SingleInstance();

            builder.RegisterType<GetHumidityCommand>().As<IWeatherCommand>().Named<IWeatherCommand>("HumidityCommand");
            builder.RegisterType<GetPressureCommand>().As<IWeatherCommand>().Named<IWeatherCommand>("PressureCommand");
            builder.RegisterType<GetTemperatureCommand>().As<IWeatherCommand>().Named<IWeatherCommand>("TemperatureCommand");
            builder.RegisterType<GetWindSpeedCommand>().As<IWeatherCommand>().Named<IWeatherCommand>("WindSpeedCommand");

            builder.RegisterDecorator<LoggingDecorator, IWeatherCommand>();
        }
    }
}

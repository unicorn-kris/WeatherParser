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

            builder.RegisterType<MainViewModel>().AsSelf();
            builder.RegisterType<DeviationsViewModel>().AsSelf();


            builder.Register<ILogger>(log =>
            {
                return new LoggerConfiguration()
                    .WriteTo.File("log.txt")
                    .CreateLogger();
            }).SingleInstance();

            builder.RegisterType<GetHumidityCommand>().As<ICommand>().Named<ICommand>("HumidityCommand");
            builder.RegisterType<GetPressureCommand>().As<ICommand>().Named<ICommand>("PressureCommand");
            builder.RegisterType<GetTemperatureCommand>().As<ICommand>().Named<ICommand>("TemperatureCommand");
            builder.RegisterType<GetWindSpeedCommand>().As<ICommand>().Named<ICommand>("WindSpeedCommand");

            builder.RegisterDecorator<LoggingDecorator, ICommand>();
        }
    }
}

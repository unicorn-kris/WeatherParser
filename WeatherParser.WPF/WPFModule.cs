using Autofac;
using WeatherParser.WPF.ViewModels;
using TimerSaveDataService;
using Grpc.Net.Client;
using Grpc.Core;
using WeatherParser.GrpcService.Services;
using Serilog;
using WeatherParser.WPF.Commands;
using WeatherParser.WPF.Decorators;

namespace WeatherParser.WPF
{
    public class WPFModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => GrpcChannel.ForAddress("http://localhost:5004")).As<ChannelBase>().SingleInstance();

            builder.RegisterType<WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient>();

            builder.RegisterType<MainWindowViewModel>().AsSelf().SingleInstance();

            builder.Register<ILogger>(log =>
            {
                return new LoggerConfiguration()
                    .WriteTo.File("log.txt")
                    .CreateLogger();
            }).SingleInstance();

            builder.RegisterModule<TimerSaveDataModule>();

            builder.RegisterType<GetHumidityCommand>().As<ICommand>().Named<ICommand>("HumidityCommand");
            builder.RegisterType<GetPressureCommand>().As<ICommand>().Named<ICommand>("PressureCommand");
            builder.RegisterType<GetTemperatureCommand>().As<ICommand>().Named<ICommand>("TemperatureCommand");
            builder.RegisterType<GetWindSpeedCommand>().As<ICommand>().Named<ICommand>("WindSpeedCommand");

            builder.RegisterDecorator<LoggingDecorator, ICommand>();
        }
    }
}

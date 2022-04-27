using Autofac;
using WeatherParser.WPF.ViewModels;
using TimerSaveDataService;
using Grpc.Net.Client;
using Grpc.Core;
using WeatherParser.GrpcService.Services;

namespace WeatherParser.WPF
{
    public class WPFModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => GrpcChannel.ForAddress("http://localhost:5004")).As<ChannelBase>().SingleInstance();

            builder.RegisterType<WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient>();

            builder.RegisterType<MainWindowViewModel>().As<IMainWindowViewModel>().SingleInstance();

            builder.RegisterModule<TimerSaveDataModule>();
        }
    }
}

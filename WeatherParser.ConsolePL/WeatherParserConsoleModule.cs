using Autofac;
using Grpc.Core;
using Grpc.Net.Client;
using WeatherParser.GrpcService.Services;

namespace WeatherParser.ConsolePL
{
    public class WeatherParserConsoleModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => GrpcChannel.ForAddress("http://localhost:5000")).As<ChannelBase>().SingleInstance();

            builder.RegisterType<WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient>();
        }
    }
}

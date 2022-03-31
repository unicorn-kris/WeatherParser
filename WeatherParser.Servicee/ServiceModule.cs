using Autofac;
using WeatherParser.Repository;
using WeatherParser.Service.Contract;

namespace WeatherParser.Service
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WeatherDataAngleSharpService>().As<IWeatherParserService>();

            builder.RegisterModule<RepositoryModule>();
        }
    }
}

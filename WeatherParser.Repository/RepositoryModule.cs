using Autofac;
using WeatherParser.Repository.Contract;

namespace WeatherParser.Repository
{
    public class RepositoryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WeatherDataNoSQLRepository>().As<IWeatherParserRepository>().SingleInstance();
        }
    }
}

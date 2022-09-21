using Autofac;
using WeatherParser.Repository;
using WeatherParser.Service.Contract;
using WeatherParser.Service.Plugins.GismeteoService;

namespace WeatherParser.Service
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Service>().As<IService>();

            //register all plugins modules
            builder.RegisterModule<GismeteoServiceModule>();

            builder.RegisterModule<RepositoryModule>();
        }
    }
}

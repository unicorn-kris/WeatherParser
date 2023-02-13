using Autofac;
using Autofac.Features.AttributeFilters;
using WeatherParser.Repository;
using WeatherParser.Service.Contract;
using WeatherParser.Service.OpenWeatherMapService;
using WeatherParser.Service.Plugins.ForecaService;
using WeatherParser.Service.Plugins.GismeteoService;

namespace WeatherParser.Service
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Service>().As<IService>().WithAttributeFiltering().SingleInstance();

            //register all plugins modules
           // builder.RegisterModule<ForecaServiceModule>();
            builder.RegisterModule<GismeteoServiceModule>();
            builder.RegisterModule<OpenWeatherMapServiceModule>();


            builder.RegisterModule<RepositoryModule>();
        }
    }
}

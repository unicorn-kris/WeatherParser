using Autofac;
using MongoDB.Driver;
using WeatherParser.Repository.Contract;

namespace WeatherParser.Repository
{
    public class RepositoryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new MongoClient("mongodb://localhost:27017")).As<IMongoClient>();

            builder.RegisterType<WeatherDataNoSQLRepository>().As<IWeatherParserRepository>().SingleInstance();
        }
    }
}

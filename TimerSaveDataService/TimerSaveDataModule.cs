using Autofac;
using WeatherParser.Service;
using WeatherParser.TimerSaveDataService;

namespace TimerSaveDataService
{
    public class TimerSaveDataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<ServiceModule>();

            builder.RegisterType<TimerSaveData>().As<ITimerSaveData>();
        }
    }
}

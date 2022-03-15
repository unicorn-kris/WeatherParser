using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherParser.Service;
using WeatherParser.TimerSaveDataService;

namespace WeatherParser.ConsolePL
{
    public class WeatherParserConsoleModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<ServiceModule>();
            builder.RegisterType<ITimerSaveData>().As<TimerSaveData>().SingleInstance();
        }
    }
}

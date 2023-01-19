using Google.Protobuf.WellKnownTypes;
using Serilog;
using System;
using System.Windows.Threading;
using WeatherParser.GrpcService.Services;

namespace WeatherParser.TimerSaveDataService
{
    public class TimerSaveData : ITimerSaveData
    {
        private WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient _weatherParserService;

        private ILogger _logger;

        public TimerSaveData(WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient weatherParserService, ILogger logger)
        {
            _weatherParserService = weatherParserService;
            _logger = logger;
        }

        public void SaveData()
        {
            var timer = new DispatcherTimer();

            SaveWeather();

            timer.Interval = TimeSpan.FromHours(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            SaveWeather();
        }

        private void SaveWeather()
        {
            try
            {
                _weatherParserService.SaveWeatherData(new Empty());
            }
            catch (Exception e)
            {
                _logger.Error($"SaveData have an error: {e.Message} StackTrace: {e.StackTrace}");
            }
        }
    }
}

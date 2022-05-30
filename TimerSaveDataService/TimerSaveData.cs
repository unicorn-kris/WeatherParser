using Serilog;
using System;
using System.Windows.Threading;
using WeatherParser.GrpcService.Services;
using WeatherParser.Presentation.Entities.Urls;

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

            timer.Interval = TimeSpan.FromDays(1);
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
                _weatherParserService.SaveWeatherData(new WeatherDataSaveRequest() { Url = UrlsSaratovGismeteo.urlWeatherToday, Day = 0 });
                _weatherParserService.SaveWeatherData(new WeatherDataSaveRequest() { Url = UrlsSaratovGismeteo.urlWeatherTomorrow, Day = 1 });
                _weatherParserService.SaveWeatherData(new WeatherDataSaveRequest() { Url = UrlsSaratovGismeteo.urlWeather3Day, Day = 2 });
                _weatherParserService.SaveWeatherData(new WeatherDataSaveRequest() { Url = UrlsSaratovGismeteo.urlWeater4Day, Day = 3 });
                _weatherParserService.SaveWeatherData(new WeatherDataSaveRequest() { Url = UrlsSaratovGismeteo.urlWeater5Day, Day = 4 });
                _weatherParserService.SaveWeatherData(new WeatherDataSaveRequest() { Url = UrlsSaratovGismeteo.urlWeater6Day, Day = 5 });
                _weatherParserService.SaveWeatherData(new WeatherDataSaveRequest() { Url = UrlsSaratovGismeteo.urlWeater7Day, Day = 6 });
            }
            catch (Exception e)
            {
                _logger.Error($"SaveData have an error: {e.Message} StackTrace: {e.StackTrace}");
            }
        }
    }
}

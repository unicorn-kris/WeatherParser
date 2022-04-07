using System;
using System.IO;
using System.Windows.Threading;
using WeatherParser.Entities.Urls;
using WeatherParser.Service.Contract;

namespace WeatherParser.TimerSaveDataService
{
    public class TimerSaveData : ITimerSaveData
    {
        private IWeatherParserService _weatherParserService;

        public TimerSaveData(IWeatherParserService weatherParserService)
        {
            _weatherParserService = weatherParserService;
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
                _weatherParserService.SaveWeatherData(UrlsSaratovGismeteo.urlWeatherToday, 0);
                _weatherParserService.SaveWeatherData(UrlsSaratovGismeteo.urlWeatherTomorrow, 1);
                _weatherParserService.SaveWeatherData(UrlsSaratovGismeteo.urlWeather3Day, 2);
                _weatherParserService.SaveWeatherData(UrlsSaratovGismeteo.urlWeater4Day, 3);
                _weatherParserService.SaveWeatherData(UrlsSaratovGismeteo.urlWeater5Day, 4);
                _weatherParserService.SaveWeatherData(UrlsSaratovGismeteo.urlWeater6Day, 5);
                _weatherParserService.SaveWeatherData(UrlsSaratovGismeteo.urlWeater7Day, 6);
            }
            catch (Exception e)
            {
                using (StreamWriter fileError = new StreamWriter("C:/MonitoringWeather/Errors.txt", true))
                {
                    fileError.WriteLine(e.Message);
                }
            }
        }
    }
}

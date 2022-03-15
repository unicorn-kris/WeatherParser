using System;
using System.IO;
using System.Windows.Threading;
using WeatherParser.Entities;
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
            DispatcherTimer timer = new DispatcherTimer();

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
                _weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeatherToday, 0);
                _weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeatherTomorrow, 1);
                _weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeather3Day, 2);
                _weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeater4Day, 3);
                _weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeater5Day, 4);
                _weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeater6Day, 5);
                _weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeater7Day, 6);
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

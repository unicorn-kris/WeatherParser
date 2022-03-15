using System;
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
            if (DateTime.Now.Hour == 1)
            {
                SaveWeather();
            }
            timer.Interval = TimeSpan.FromHours(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now.Hour == 1)
                SaveWeather();
        }

        private void SaveWeather()
        {
            try
            {
                var resultToday = _weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeatherToday, 0);
                var resultTomorrow = _weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeatherTomorrow, 1);
                var result3Day = _weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeather3Day, 2);
                var result4Day = _weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeater4Day, 3);
                var result5Day = _weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeater5Day, 4);
                var result6Day = _weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeater6Day, 5);
                var result7Day = _weatherParserService.SaveWeatherData(MainUrlSaratov.urlWeater7Day, 6);
            }
            catch
            {
                Console.WriteLine("Error");
            }
        }
    }
}

using AngleSharp;
using System.Reflection;
using WeatherParser.Service.Common;
using WeatherParser.Service.Entities;
using WeatherParser.Service.Entities.Urls;

namespace WeatherParser.Service.Plugins.GismeteoService
{
    public class WeatherDataAngleSharpServiceGismeteo : IWeatherPlugin
    {
        public Guid SiteID => new Guid("ed13908a-c2dc-4edb-bb9c-1678300a3435");

        public string Name => "Gismeteo";

        public Task<WeatherDataService> SaveWeatherDataAsync()
        {
            var weatherDataList = new List<WeatherService>();

            var urls = new List<string>();

            //find all static fields
            FieldInfo[] fields = typeof(GismeteoSaratovCollection).GetFields(BindingFlags.Static | BindingFlags.Public);

            //bring in collection
            foreach (FieldInfo field in fields)
            {
                urls.Add((string)field.GetValue(null));
            }

            foreach (string url in urls)
            {
                var config = Configuration.Default.WithDefaultLoader();
                var doc = BrowsingContext.New(config).OpenAsync(url);
                var parsedHtml = doc.Result;

                //for checking parsed html
                //var html = parsedHtml.Body.OuterHtml;
                //File.WriteAllText("log.txt", html);

                var temperatures = parsedHtml.GetElementsByClassName("widget-row-chart widget-row-chart-temperature");
                var windSpeeds = parsedHtml.GetElementsByClassName("widget-row widget-row-wind-speed-gust row-with-caption");
                var windDirections = parsedHtml.GetElementsByClassName("widget-row widget-row-wind-direction");
                var pressures = parsedHtml.GetElementsByClassName("widget-row-chart widget-row-chart-pressure");
                var humidities = parsedHtml.GetElementsByClassName("widget-row widget-row-humidity");
                var date = parsedHtml.GetElementsByClassName("tab-content");

                WeatherService weatherData = new WeatherService()
                {
                    Temperature = new List<double>(),
                    Humidity = new List<double>(),
                    Pressure = new List<double>(),
                    WindDirection = new List<string>(),
                    WindSpeed = new List<double>()
                };

                var day = int.Parse(string.Join("", date[1].GetElementsByClassName("date")[0].TextContent.Trim().Where(c => char.IsDigit(c))));
                if (day < DateTime.Now.Day)
                {
                    weatherData.Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month < 12 ? DateTime.Now.Month + 1 : 1, day);
                }
                else
                {
                    weatherData.Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, day);
                }

                for (int i = 0; i < 8; ++i)
                {
                    string temperature = temperatures[0]
                        .GetElementsByClassName("chart")[0]
                        .GetElementsByClassName("values")[0]
                        .GetElementsByClassName("value")[i]
                        .QuerySelector("span").TextContent.Trim();

                    if (temperature.Any(c => c == '−'))
                    {
                        weatherData.Temperature.Add(double.Parse(temperature.Replace('−', ' ').Trim()) * -1);
                    }
                    else
                    {
                        weatherData.Temperature.Add(double.Parse(temperature));
                    }

                    string windSpeed = windSpeeds[0]
                        .GetElementsByClassName("row-item")[i]
                        .QuerySelectorAll("span")[0].TextContent.Trim();

                    if (windSpeed.Any(c => c == '-'))
                    {
                        weatherData.WindSpeed.Add(int.Parse(windSpeed.Split('-')[0]));
                    }
                    else
                    {
                        weatherData.WindSpeed.Add(int.Parse(windSpeed));
                    }

                    if (windSpeed != "0")
                    {
                        weatherData.WindDirection.Add(windDirections[0]
                             .GetElementsByClassName("row-item")[i]
                             .GetElementsByClassName("direction")[0].TextContent.Trim());
                    }

                    weatherData.Pressure.Add(int.Parse(pressures[0]
                        .GetElementsByClassName("chart")[0]
                        .GetElementsByClassName("values")[0]
                        .GetElementsByClassName("value")[i]
                        .QuerySelectorAll("span")[0].TextContent.Trim()));

                    weatherData.Humidity.Add(int.Parse(humidities[0]
                        .QuerySelectorAll("div")[i].TextContent.Trim()));
                }

                weatherDataList.Add(weatherData);
            }

            //map service entity to repository entity
            return Task.FromResult(new WeatherDataService()
            {
                TargetDate = DateTime.Now,
                Weather = weatherDataList,
                SiteId = SiteID
            });
        }
    }
}
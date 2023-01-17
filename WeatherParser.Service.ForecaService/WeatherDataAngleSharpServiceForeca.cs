using AngleSharp;
using Helpers;
using System.Reflection;
using WeatherParser.Service.Entities;
using WeatherParser.Service.Entities.Urls;
using WeatherParser.Service.GismeteoService.Contract;

namespace WeatherParser.Service.Plugins.GismeteoService
{
    public class WeatherDataAngleSharpServiceForeca : IWeatherParserServiceForeca
    {
        public List<WeatherDataService> SaveWeatherData()
        {
            var resultListWeatherDataService = new List<WeatherDataService>();

            var urls = new List<string>();

            //find all static fields
            FieldInfo[] fields = typeof(ForecaSaratovCollection).GetFields(BindingFlags.Static);

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

                //var html = parsedHtml.Body.OuterHtml;
                //File.WriteAllText("log.txt", html);

                var weathers = parsedHtml.GetElementsByClassName("row clr5");

                WeatherService weatherData = new WeatherService()
                {
                    Temperature = new List<double>(),
                    Humidity = new List<int>(),
                    Pressure = new List<int>(),
                    WindDirection = new List<string>(),
                    WindSpeed = new List<int>()
                };

                //TODO PARSE DATE
                weatherData.Date = DateTime.UtcNow.AddDays(1);

                for (int i = 0; i < 8; ++i)
                {
                    string temperature = weathers[i*3 + 1]
                        .GetElementsByClassName("c4")[0]
                        .QuerySelector("strong").TextContent.Trim();

                    if (temperature.Any(c => c == '−'))
                    {
                        weatherData.Temperature.Add(double.Parse(temperature.Replace('−', ' ').Trim()) * -1);
                    }
                    else
                    {
                        weatherData.Temperature.Add(double.Parse(temperature));
                    }

                    string windSpeed = weathers[i * 3 + 1]
                        .GetElementsByClassName("c2")[0]
                        .QuerySelector("strong").TextContent.Trim();

                    if (windSpeed.Any(c => c == '-'))
                    {
                        weatherData.WindSpeed.Add(int.Parse(windSpeed.Split('-')[0]));
                    }
                    else
                    {
                        weatherData.WindSpeed.Add(int.Parse(windSpeed));
                    }

                    weatherData.Pressure.Add(int.Parse(weathers[i * 3 + 1]
                        .GetElementsByClassName("c3")[0]
                        .GetAttribute("strong")[2].ToString().Trim()));
                }

                //map service entity to repository entity
                var newWeatherDataRepository = new WeatherDataService()
                {
                    TargetDate = DateTime.UtcNow,
                    Weather = new List<WeatherService>() { weatherData },
                    SiteId = SitesHelperCollection.GismeteoSaratovCollection
                };

                resultListWeatherDataService.Add(newWeatherDataRepository);
            }
            return resultListWeatherDataService;
        }
    }
}
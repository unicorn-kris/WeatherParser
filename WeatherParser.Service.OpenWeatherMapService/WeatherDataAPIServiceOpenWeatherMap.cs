using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WeatherParser.Service.Common;
using WeatherParser.Service.Entities;
using WeatherParser.Service.OpenWeatherMapService.ResponseEntity;

namespace WeatherParser.Service.OpenWeatherMapService
{
    public class WeatherDataAPIServiceOpenWeatherMap : IWeatherPlugin
    {
        public Guid SiteID => new Guid("e8c192f8-1ec2-4976-880a-449863e072b0");

        public string Name => "OpenWeatherMap";

        private HttpClient _httpClient;

        private readonly string _openWeatherMapToken = "3c4d61f32c0c6ebfe830af8d86c96a9a";
        private readonly string _saratovID = "498677";

        public WeatherDataAPIServiceOpenWeatherMap(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<WeatherDataService> SaveWeatherDataAsync()
        {
            var weatherData = await GetWeatherAsync(_saratovID);

            var weatherDataList = new List<WeatherService>();

            foreach (var weather in weatherData.List)
            {
                var targetDate = DateTime.Parse(weather.Dt_txt).Date;

                if (!weatherDataList.Any(x => x.Date == targetDate))
                {
                    weatherDataList.Add(new WeatherService()
                    {
                        Date = targetDate,
                        Humidity = new List<double>(),
                        Pressure = new List<double>(),
                        Temperature = new List<double>(),
                        WindSpeed = new List<double>(),
                        Hours = new List<int>()
                    });
                }

                weatherDataList.FirstOrDefault(x => x.Date == targetDate).Pressure.Add(weather.Main.Pressure);
                weatherDataList.FirstOrDefault(x => x.Date == targetDate).Humidity.Add(weather.Main.Humidity);
                //from K to C
                weatherDataList.FirstOrDefault(x => x.Date == targetDate).Temperature.Add(weather.Main.Temp - 273.15);
                weatherDataList.FirstOrDefault(x => x.Date == targetDate).WindSpeed.Add(weather.Wind.Speed);
                weatherDataList.FirstOrDefault(x => x.Date == targetDate).Hours.Add(DateTime.Parse(weather.Dt_txt).Hour);
            }

            return new WeatherDataService() { SiteId = SiteID, TargetDate = DateTime.Now.Date, Weather = weatherDataList };
        }

        private async Task<OpenWeatherResponse> GetWeatherAsync(string cityId)
        {
            var response = await _httpClient.GetAsync($"http://api.openweathermap.org/data/2.5/forecast?id={cityId}&appid={_openWeatherMapToken}");

            var serializer = new JsonSerializer() { ContractResolver = new DefaultContractResolver() { NamingStrategy = new CamelCaseNamingStrategy() } };

            var responseContent = await response.Content.ReadAsStringAsync();
            using var responseStringReader = new StringReader(responseContent);
            using var responseJsonReader = new JsonTextReader(responseStringReader);

            return serializer.Deserialize<OpenWeatherResponse>(responseJsonReader);
        }
    }
}
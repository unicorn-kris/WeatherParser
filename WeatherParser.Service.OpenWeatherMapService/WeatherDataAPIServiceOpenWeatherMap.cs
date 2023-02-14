using Newtonsoft.Json;
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

            foreach (var weather in weatherData.WeatherData) {
                //TODO add weather data
                    }

            return null;
        }

        private async Task<OpenWeatherResponse> GetWeatherAsync(string cityId)
        {
                var response = await _httpClient.GetAsync($"api.openweathermap.org/data/2.5/forecast?id={cityId}&appid={_openWeatherMapToken}");

                var responseContent = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<OpenWeatherResponse>(responseContent);
        }
    }
}
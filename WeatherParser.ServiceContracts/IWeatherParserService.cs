namespace WeatherParser.ServiceContracts
{
    public interface IWeatherParserService
    {
        string GetDataAsync(string url);
        string GetTemperature(string url);
        string GetPressure(string url);
        string GetWindSpeed(string url);
        string GetWindDirection(string url);
        string GetHumidity(string url);

    }
}

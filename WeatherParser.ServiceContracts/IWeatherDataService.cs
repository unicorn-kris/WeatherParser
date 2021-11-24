namespace WeatherParser.ServiceContracts
{
    public interface IWeatherDataService
    {
        string GetData(string url);
        string GetTemperature(string url);
        string GetPressure(string url);
        string GetWindSpeed(string url);
        string GetWindDirection(string url);
        string GetHumidity(string url);

    }
}

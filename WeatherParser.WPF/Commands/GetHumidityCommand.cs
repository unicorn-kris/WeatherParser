using WeatherParser.Presentation.Entities;

namespace WeatherParser.WPF.Commands
{
    internal class GetHumidityCommand : WeatherCommandBase
    {
        public override double AddData(WeatherPresentation weatherPresentation, int index)
        {
            return weatherPresentation.Humidity[index];
        }
    }
}

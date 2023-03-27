using WeatherParser.Presentation.Entities;

namespace WeatherParser.WPF.Commands
{
    internal class GetTemperatureCommand : WeatherCommandBase
    {
        public override double AddData(WeatherPresentation weatherPresentation, int index)
        {
            return weatherPresentation.Temperature[index];
        }
    }
}

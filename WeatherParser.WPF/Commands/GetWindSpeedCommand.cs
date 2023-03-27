using WeatherParser.Presentation.Entities;

namespace WeatherParser.WPF.Commands
{
    internal class GetWindSpeedCommand : WeatherCommandBase
    {
        public override double AddData(WeatherPresentation weatherPresentation, int index)
        {
            return weatherPresentation.WindSpeed[index];
        }
    }
}

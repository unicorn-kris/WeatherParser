using WeatherParser.Presentation.Entities;

namespace WeatherParser.WPF.Commands
{
    internal class GetPressureCommand : WeatherCommandBase
    {
        public override double AddData(WeatherPresentation weatherPresentation, int index)
        {
            return weatherPresentation.Pressure[index];
        }
    }
}

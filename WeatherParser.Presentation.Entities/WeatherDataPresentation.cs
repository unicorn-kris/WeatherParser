using System;
using System.Collections.Generic;

namespace WeatherParser.Presentation.Entities
{
    public class WeatherDataPresentation
    {
        public DateTime TargetDate { get; set; }

        public List<WeatherPresentation> Weather { get; set; }
    }
}

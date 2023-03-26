using System;
using System.Threading.Tasks;
using WeatherParser.GrpcService.Services;
using WeatherParser.Presentation.Entities;
using WeatherParser.WPF.ViewModels.Contract;

namespace WeatherParser.WPF.ViewModels
{
    internal class MeanDeviationsViewModel : DeviationsViewModel
    {
        public int DaysCount { get; set; }

        public override async Task GetWeatherAsync(WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient weatherDataProtoGismeteo,
            SitePresentation selectedSite,
            DateTime? selectedDate)
        {
            this.WeatherDataPresentations = CastToPresentationEntity(await weatherDataProtoGismeteo.GetMeanDeviationsOfRealForecastAsync(new GetMeanDeviationsRequest()
            {
                SiteID = selectedSite.ID.ToString(),
                Days = 3
            }));
        }
    }
}

using Google.Protobuf.WellKnownTypes;
using System;
using System.Threading.Tasks;
using WeatherParser.GrpcService.Services;
using WeatherParser.Presentation.Entities;
using WeatherParser.WPF.ViewModels.Contract;

namespace WeatherParser.WPF.ViewModels
{
    internal class ForecastViewModel : DeviationsViewModel
    {
        public override async Task GetWeatherAsync(WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient weatherDataProtoGismeteo,
            SitePresentation selectedSite,
            DateTime? selectedDate)
        {
            this.WeatherDataPresentations = CastToPresentationEntity(await weatherDataProtoGismeteo.GetAllWeatherDataByDayAsync(new WeatherDataRequest()
            {
                Date = DateTime.SpecifyKind((DateTime)selectedDate, DateTimeKind.Utc).ToTimestamp(),
                SiteID = selectedSite.ID.ToString()
            }));
        }
    }
}

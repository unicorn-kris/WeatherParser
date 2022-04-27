using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using WeatherParser.Service.Contract;
using WeatherParser.Service.Entities;

namespace WeatherParser.GrpcService.Services
{
    public class WeatherDataGismeteoService : WeatherDataProtoGismeteo.WeatherDataProtoGismeteoBase
    {
        private readonly ILogger<WeatherDataGismeteoService> _logger;
        private readonly IWeatherParserServiceGismeteo _weatherParserService;

        public WeatherDataGismeteoService(ILogger<WeatherDataGismeteoService> logger, IWeatherParserServiceGismeteo weatherParserServiceGismeteo)
        {
            _logger = logger;
            _weatherParserService = weatherParserServiceGismeteo;
        }

        public override Task<WeatherDataGetResponse> GetAllWeatherData(Timestamp request, ServerCallContext context)
        {
            try
            {
                Dictionary<DateTime, List<WeatherDataService>> weatherData = _weatherParserService.GetAllWeatherData(request.ToDateTime());

                var returnWeatherDataDict = new WeatherDataGetResponse();

                foreach (var newWeatherData in weatherData)
                {
                    var newList = new weatherDataList();

                    foreach (var item in newWeatherData.Value)
                    {
                        newList.WeatherDataList.Add(new WeatherDataProto()
                        {
                            CollectionDate = item.CollectionDate.ToUniversalTime().ToTimestamp(),
                            Date = item.Date.ToUniversalTime().ToTimestamp(),
                            Temperature = item.Temperature,
                            Humidity = item.Humidity,
                            Pressure = item.Pressure,
                            WindDirection = item.WindDirection,
                            WindSpeedFirst = item.WindSpeedFirst,
                            WindSpeedSecond = item.WindSpeedSecond,
                        });
                    }

                    returnWeatherDataDict.WeatherDataDictionary.Add(new KeyValuePair()
                    {
                        Key = newWeatherData.Key.ToUniversalTime().ToTimestamp(),
                        Value = newList
                    });
                }

                return Task.FromResult(returnWeatherDataDict);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllWeatherData failed");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override Task<Timestamp> GetFirstDate(Empty request, ServerCallContext context)
        {
            try
            {
                return Task.FromResult(_weatherParserService.GetFirstDate().ToTimestamp());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetFirstDate failed");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override Task<Timestamp> GetLastDate(Empty request, ServerCallContext context)
        {
            try
            {
                return Task.FromResult(_weatherParserService.GetLastDate().ToTimestamp());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetLastDate failed");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        public override Task<Empty> SaveWeatherData(WeatherDataSaveRequest request, ServerCallContext context)
        {
            try
            {
                _weatherParserService.SaveWeatherData(request.Url, request.Day);
                return Task.FromResult(new Empty());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SaveWeatherData failed");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }
    }
}
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using TongBuilder.Contract.Contracts;
using TongBuilder.Contract.Core;
using TongBuilder.Contract.Models;

namespace TongBuilder.Application.Business
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _client;
        private readonly ILogger<WeatherService> _logger = null!;

        public WeatherService(IHttpClientFactory httpClientFactory, ILogger<WeatherService> logger)
        {
            _logger = logger;
            try
            {
                _client = httpClientFactory.CreateClient("TongBuilderProxy");
            }
            catch (Exception ex)
            {
                _logger.LogError("CreateClient TongBuilderProxy: {Error}", ex);                
            }

        }

        public void DeleteWeather(WeatherForecast weather)
        {
            throw new NotImplementedException();
        }

        public async Task<WeatherForecast[]?> GetWeather()
        {
            try
            {
                var ret = await _client.GetFromJsonAsync<WeatherForecast[]?>("/api/WeatherForecast/GetWeather");
                return ret;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetWeather: {Error}", ex);
                return null;
            }

        }

        public PagingResult<WeatherForecast> QueryWeathers(PagingCriteria criteria)
        {
            throw new NotImplementedException();
        }

        public void SaveWeather(WeatherForecast weather)
        {
            throw new NotImplementedException();
        }
    }
}

using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using TongBuilder.Contract.Contracts;
using TongBuilder.Contract.Models;

namespace TongBuilder.Application.Business
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _client;
        private readonly ILogger<WeatherService> _logger = null!;

        public WeatherService(IHttpClientFactory httpClientFactory, ILogger<WeatherService> logger)
        {
            _client = httpClientFactory.CreateClient("TongBuilderProxy");
            _logger = logger;
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

    }
}

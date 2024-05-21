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
        //创建静态变量存储天气数据
        static readonly List<WeatherForecast> forecasts = [];
        public readonly string[] summaries =
   [
       "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
   ];

        static WeatherService()
        {
            var startDate = DateOnly.FromDateTime(DateTime.Now);
            var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
            forecasts = Enumerable.Range(1, 10).Select(index => new WeatherForecast
            {
                Id = index,
                Date = startDate.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = summaries[Random.Shared.Next(summaries.Length)]
            }).ToList();
        }

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
            var forecast = forecasts.FirstOrDefault(f => f.Date == weather.Date);
            if (forecast == null)
                return;

            forecasts.Remove(forecast);
        }

        public async Task<WeatherForecast[]?> GetWeather()
        {
            //try
            //{
            //    var ret = await _client.GetFromJsonAsync<WeatherForecast[]?>("/api/WeatherForecast/GetWeather");
            //    return ret;
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError("GetWeather: {Error}", ex);
            //    return null;
            //}
            //await Task.Delay(500);           
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Id = index,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),               
                Summary = summaries[Random.Shared.Next(summaries.Length)]
            })
            .ToArray();
        }

        public PagingResult<WeatherForecast> QueryWeathers(PagingCriteria criteria)
        {
            var queryDatas = forecasts;
            if (criteria.Parameters.ContainsKey("Date"))
            {
                var dates = criteria.Parameters["Date"] as DateTime?[];
                if (dates != null && dates[0] != null && dates[1] != null)
                {
                    var start = DateOnly.FromDateTime(dates[0].Value);
                    var end = DateOnly.FromDateTime(dates[1].Value);
                    queryDatas = forecasts.Where(f => f.Date >= start && f.Date <= end).ToList();
                }
            }

            var pageData = queryDatas.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize).ToList();
            return new PagingResult<WeatherForecast>(forecasts.Count, pageData);
        }

        public void SaveWeather(WeatherForecast weather)
        {
            var forecast = forecasts.FirstOrDefault(f => f.Date == weather.Date);
            if (forecast == null)
            {
                forecasts.Add(weather);
                return;
            }

            forecast.TemperatureC = weather.TemperatureC;
            forecast.Summary = weather.Summary;
        }
    }
}

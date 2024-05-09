using TongBuilder.Contract.Models;

namespace TongBuilder.BlazorWeb.Services
{
    public class WeatherForecastService : IWeatherForecastService
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot"
    };

        public async Task<WeatherForecast[]?> GetForecastAsync(DateTime startDate)
        {
            var ret = await Task.FromResult(Enumerable.Range(1, 5)
                .Select(index => new WeatherForecast
                {
                    Date = startDate.AddDays(index),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                }).ToArray());
            return ret;
        }
            
    }
}

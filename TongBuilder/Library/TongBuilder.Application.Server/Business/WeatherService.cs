using TongBuilder.Contract.Contracts;
using TongBuilder.Contract.Models;

namespace TongBuilder.Application.Server.Business
{
    public class WeatherService : IWeatherService
    {
        public readonly string[] summaries =
   [
       "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
   ];

        public async Task<WeatherForecast[]?> GetWeather()
        {
            await Task.Delay(500);

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = summaries[Random.Shared.Next(summaries.Length)]
            })
            .ToArray();

        }

    }
}

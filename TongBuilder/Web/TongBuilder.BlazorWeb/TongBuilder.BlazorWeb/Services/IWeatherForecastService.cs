using TongBuilder.Contract.Models;

namespace TongBuilder.BlazorWeb.Services
{
    public interface IWeatherForecastService
    {
        Task<WeatherForecast[]?> GetForecastAsync(DateTime startDate);
    }
}

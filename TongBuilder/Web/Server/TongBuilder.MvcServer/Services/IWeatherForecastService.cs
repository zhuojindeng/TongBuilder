using TongBuilder.Contract.Models;

namespace TongBuilder.MvcServer.Services
{
    public interface IWeatherForecastService
    {
        Task<WeatherForecast[]?> GetForecastAsync(DateTime startDate);
    }
}

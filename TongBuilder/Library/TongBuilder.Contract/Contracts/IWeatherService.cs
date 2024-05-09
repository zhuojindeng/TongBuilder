using TongBuilder.Contract.Models;

namespace TongBuilder.Contract.Contracts
{
    public interface IWeatherService
    {
        Task<WeatherForecast[]?> GetWeather();
    }
}

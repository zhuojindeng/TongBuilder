using TongBuilder.Contract.Core;
using TongBuilder.Contract.Models;

namespace TongBuilder.Contract.Contracts
{
    public interface IWeatherService
    {
        Task<WeatherForecast[]?> GetWeather();

        /// <summary>
        /// 根据日期范围查询天气
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        PagingResult<WeatherForecast> QueryWeathers(PagingCriteria criteria);

        void SaveWeather(WeatherForecast weather);

        void DeleteWeather(WeatherForecast weather);
    }
}

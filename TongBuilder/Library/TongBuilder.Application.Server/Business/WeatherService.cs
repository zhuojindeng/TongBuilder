using TongBuilder.Contract.Contracts;
using TongBuilder.Contract.Core;
using TongBuilder.Contract.Models;

namespace TongBuilder.Application.Server.Business
{
    public class WeatherService : IWeatherService
    {
        public readonly string[] summaries =
   [
       "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
   ];
        //创建静态变量存储天气数据
        static readonly List<WeatherForecast> forecasts = [];

        //默认预测10天的天气数据
        static WeatherService()
        {
            var startDate = DateOnly.FromDateTime(DateTime.Now);
            var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
            forecasts = Enumerable.Range(1, 10).Select(index => new WeatherForecast
            {
                Date = startDate.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = summaries[Random.Shared.Next(summaries.Length)]
            }).ToList();
        }


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

        //根据日期范围查询天气
        // 修改查询方法
        // - 参数改成查询条件类
        // - 返回值改成查询结果类
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

        //保存天气数据
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

        //删除天气数据
        public void DeleteWeather(WeatherForecast weather)
        {
            var forecast = forecasts.FirstOrDefault(f => f.Date == weather.Date);
            if (forecast == null)
                return;

            forecasts.Remove(forecast);
        }
    }
}

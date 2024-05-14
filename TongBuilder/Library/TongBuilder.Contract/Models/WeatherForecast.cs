using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace TongBuilder.Contract.Models
{
    public class WeatherForecast
    {
        [DisplayName("日期")]
        [Required(ErrorMessage = "日期不能为空！")]
        public DateOnly Date { get; set; }

        [DisplayName("温度(C)")]
        [Required(ErrorMessage = "温度不能为空！")]
        public int TemperatureC { get; set; }

        [DisplayName("温度(F)")]
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        [DisplayName("摘要")]
        public string? Summary { get; set; }
    }
}

using AntDesign;

namespace TongBuilder.RazorLib.Constant
{
    public class UiContant
    {
        /// <summary>
        /// 本地化支持语言
        /// </summary>
        public readonly static string[] SupportedCultures = { "zh-CN", "en-US" };

        /// <summary>
        /// 本地化浏览器缓存key
        /// </summary>
        public readonly static string BlazorCultureKey = "TongBlazorCulture";

        /// <summary>
        /// 本地化默认语言
        /// </summary>
        public readonly static string DefaultCulture = "zh-CN";

        /// <summary>
        /// 时间显示格式化
        /// </summary>
        public readonly static string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";


        /// <summary>
        /// 时间显示格式化
        /// </summary>
        public readonly static string InputDateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss zzz";

        /// <summary>
        /// 每页数据量大小
        /// </summary>
        public readonly static int PageSize = 15;

        /// <summary>
        /// 通知消息弹出时长
        /// </summary>
        public readonly static int ClientNotifierMessageDuration = 3;

        /// <summary>
        /// 启用多标签
        /// </summary>
        public readonly static bool EnabledTabs = true;

        /// <summary>
        /// 默认表格大小
        /// </summary>
        /// <remarks>
        /// Default
        /// Middle
        /// Small
        /// </remarks>
        public readonly static TableSize DefaultTableSize = TableSize.Small;

    }
}

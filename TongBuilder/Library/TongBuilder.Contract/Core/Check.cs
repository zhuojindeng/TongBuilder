namespace TongBuilder.Contract.Core
{
    /// <summary>
    /// 类型检测
    /// </summary>
    public static class Check
    {
        /// <summary>
        /// 判断参数是否为空
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="value">参数值</param>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数值</returns>
        public static T NotNull<T>(T value, string parameterName) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }
            return value;
        }

        /// <summary>
        /// 判断参数是否为空
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="value">参数值</param>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数值</returns>
        public static T? NotNull<T>(T? value, string parameterName) where T : struct
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }
            return value;
        }

        /// <summary>
        /// 判断字符串参数是否为空
        /// </summary>
        /// <param name="value">参数值</param>
        /// <param name="parameterName">参数名称</param>
        /// <returns>参数值</returns>
        public static string NotEmpty(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"Argument {parameterName} Is Null Or Whitespace");
            }
            return value;
        }
    }
}

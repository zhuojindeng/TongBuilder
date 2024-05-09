namespace TongBuilder.SSOServer.Models
{
    /// <summary>
    /// 租户
    /// </summary>
    public class UserTenant
    {
        /// <summary>
        /// 租户名称（代码使用）
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 租户名称（前端显示）
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否默认租户
        /// </summary>
        public bool IsDefault { get; set; }
    }
}

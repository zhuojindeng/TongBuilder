
namespace TongBuilder.Contract.Models
{
    /// <summary>
    /// 用户管理服务返回的查询第三方unionid结果
    /// </summary>
    public class ExternalUser
    {
        /// <summary>
        /// 用户唯一标识
        /// </summary>
        public string? OpenId { get; set; }
        /// <summary>
        /// 第三方唯一标识
        /// </summary>
        public string? ProviderKey { get; set; }
        /// <summary>
        /// 第三方平台名称
        /// </summary>
        public string? LoginProvider { get; set; }
        /// <summary>
        /// 绑定时间
        /// </summary>
        public string? BindTime { get; set; }
    }
}

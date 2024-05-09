namespace TongBuilder.SSOServer.Models
{
    /// <summary>
    /// 手机验证码记录
    /// </summary>
    public class SmsCodeRecord
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string? PhoneNumber { get; set; }
        /// <summary>
        /// 验证码创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 验证码过期时间
        /// </summary>
        public DateTime ExpirationTime { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string? Code { get; set; }
        /// <summary>
        /// 请求次数
        /// </summary>
        public int Version { get; set; } = 0;
        /// <summary>
        /// 是否已验证，0=未验证过，1=登录验证过，2=租户选择后验证过
        /// </summary>
        public int IsVerified { get; set; }
    }
}

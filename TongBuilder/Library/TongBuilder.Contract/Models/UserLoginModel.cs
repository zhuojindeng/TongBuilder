namespace TongBuilder.Contract.Models
{
    public class UserLoginModel
    {
        public string LoginMode { get; set; } = "PhoneNumberWithPassword";

        public string? PhoneNumber { get; set; }

        public string? UserName { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string? VerificationCode { get; set; }

        /// <summary>
        /// 第三方登录类型
        /// </summary>
        public string? LoginProvider { get; set; }

        /// <summary>
        /// 第三方登录标识
        /// </summary>
        public string? ProviderKey { get; set; }

        public bool RememberMe { get; set; }
    }
}

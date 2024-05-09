using System.ComponentModel.DataAnnotations;

namespace TongBuilder.RazorLib.ViewModels
{
    /// <summary>
    /// 登录页面数据模型
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// 用户码
        /// </summary>
        [Required]
        public string? Username { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        public string? Password { get; set; }
        /// <summary>
        /// 验证后回调地址
        /// </summary>
        public string? ReturnUrl { get; set; }
        /// <summary>
        /// 第三方验证身份码
        /// </summary>
        public string? AuthCode { get; set; }
        /// <summary>
        /// 认证名称：0=Siweisoft、1=DingTalk、2=WeChat
        /// </summary>
        public string? AuthCodeType { get; set; }
        /// <summary>
        /// 登录方式：1=dingtalk, 2=wechat, 3=mobilephone, 4=password
        /// </summary>
        public string? LoginType { get; set; }
        /// <summary>
        /// 租户选择
        /// </summary>
        public string? Tenant { get; set; }
    }
}

using System.Diagnostics.CodeAnalysis;
using TongBuilder.Contract.Enums;

namespace TongBuilder.Contract.Models
{
    public class UserInfo
    {
        [AllowNull]
        public string OpenId { get; set; }

        public UserType UserType { get; set; } = UserType.Normal;

        public string? UserName { get; set; }

        public string? NickName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? TenantCode { get; set; }

        public string? TenantName { get; set; }

        /// <summary>
        /// 锁定时间
        /// </summary>
        public DateTimeOffset? LockoutEnd { get; set; }

        /// <summary>
        /// 最近一次登录时间
        /// </summary>
        public DateTimeOffset? LastLoginTime { get; set; }

        /// <summary>
        /// 最近一次登录Ip
        /// </summary>
        public string? LastLoginIp { get; set; }

        /// <summary>
        /// 最近一次登录地址
        /// </summary>
        public string? LastLoginAddress { get; set; }

        /// <summary>
        /// 最近一次尝试登录时间
        /// </summary>
        public DateTimeOffset? LastTryLoginTime { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool Enabled { get; set; }
    }
}

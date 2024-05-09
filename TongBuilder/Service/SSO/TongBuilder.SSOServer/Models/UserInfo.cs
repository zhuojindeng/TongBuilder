using System.Diagnostics.CodeAnalysis;
using TongBuilder.Contract.Models;

namespace TongBuilder.SSOServer.Models
{
    /// <summary>
    /// 用户管理服务返回的基本用户信息类
    /// </summary>
    public class UserInfo
    {
        [AllowNull]
        public string OpenId { get; set; }

        public string? NickName { get; set; }

        public string? PhoneNumber { get; set; }
    }

    /// <summary>
    /// 用户管理服务返回的包含租户列表的用户信息类
    /// </summary>
    public class UserInfoResult : UserInfo
    {
        public List<TenantModel> Tenants { get; set; }

        public bool IsSuccess { get; set; }

        public string? Tenant { get; set; }

        public string? Description { get; set; }
    }
}

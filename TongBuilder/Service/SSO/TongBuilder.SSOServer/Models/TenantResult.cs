using System.Diagnostics.CodeAnalysis;

namespace TongBuilder.SSOServer.Models
{
    /// <summary>
    /// 用户管理服务返回的租户列表
    /// </summary>
    public class TenantResult
    {
        /// <summary>
        /// 租户ID号
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 行版本
        /// </summary>
        [AllowNull]
        public byte[] RowVersion { get; set; }
        /// <summary>
        /// 租户代号
        /// </summary>
        [AllowNull]
        public string Code { get; set; }
        /// <summary>
        /// 租户名称（主要使用的是这个）
        /// </summary>
        [AllowNull]
        public string Name { get; set; }
        /// <summary>
        /// 是否为默认租户
        /// </summary>
        public bool IsDefault { get; set; }

    }

    /// <summary>
    /// 用户列表返回的详细租户类，转发用户获取租户列表接口使用
    /// </summary>
    public class TenantDto : TenantResult
    {
        public string State { get; set; }

        public DateTime CreationTime { get; set; }

        public string OriginalCreatedType { get; set; }

        public long OwnerId { get; set; }

        public string OwnerCode
        {
            get; [param: AllowNull]
            set;
        }

        public string OwnerName
        {
            get; [param: AllowNull]
            set;
        }
    }
}

using TongBuilder.Contract.Models;

namespace TongBuilder.SSOServer.ViewModels
{
    /// <summary>
    /// 租户选择页面数据模型
    /// </summary>
    public class TenantViewModel
    {
        /// <summary>
        /// 租户列表
        /// </summary>
        public List<TenantModel>? TenantList { get; set; }
        /// <summary>
        /// 应用系统名称
        /// </summary>
        public string? ApplicationName { get; set; }
        
    }
}

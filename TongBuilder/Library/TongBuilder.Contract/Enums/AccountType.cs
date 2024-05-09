namespace TongBuilder.Contract.Enums
{
    /// <summary>
    /// 用户的角色
    /// </summary>
    public enum AccountType
    {
        /// <summary>
        /// 未确定
        /// </summary>
        None = 0,

        /// <summary>
        /// 内部员工
        /// </summary>
        Employee = 1,

        /// <summary>
        /// 客户
        /// </summary>
        Customer = 2,

        /// <summary>
        /// 供应商
        /// </summary>
        Vender = 3,

        /// <summary>
        /// 委外加工单位
        /// </summary>
        Outside = 4,

        /// <summary>
        /// 代理商
        /// </summary>
        Agent = 5,

        /// <summary>
        /// 系统管理员
        /// </summary>
        SystemAdmin = 6
    }
}

namespace TongBuilder.Contract.Models
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public class SystemConfig
    {
        /// <summary>
        /// Copyright内容
        /// </summary>
        public string Copyright { get; set; } = null!;
        /// <summary>
        /// 系统名称
        /// </summary>
        public string SystemName { get; set; } = null!;
        /// <summary>
        /// 系统描述
        /// </summary>
        public string SystemDescription { get; set; } = null!;
    }
}

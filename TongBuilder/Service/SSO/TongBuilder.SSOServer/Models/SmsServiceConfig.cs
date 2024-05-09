namespace TongBuilder.SSOServer.Models
{
    /// <summary>
    /// 短信服务配置
    /// </summary>
    public class SmsServiceConfig
    {
        /// <summary>
        /// 短信签名
        /// </summary>
        public string MessageSignature { get; set; }
        /// <summary>
        /// 短信模板
        /// </summary>
        public string MessageTemplateId { get; set; }
        /// <summary>
        /// 应用ID
        /// </summary>
        public string AccessKeyId { get; set; }
        /// <summary>
        /// 应用Secret
        /// </summary>
        public string AccessKeySecret { get; set; }
    }
}

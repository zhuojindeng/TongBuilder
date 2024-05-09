namespace TongBuilder.Contract.Models
{
    /// <summary>
    /// 通用请求返回结果类
    /// </summary>
    public class ResultModel<T>: BaseResultModel<T>
    {
        /// <summary>
        /// 说明信息
        /// </summary>
        public string? Message { get; set; }
    }    

    /// <summary>
    /// 通用请求返回结果类基础
    /// </summary>
    public class BaseResultModel<T>
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Succeeded { get; set; }
        /// <summary>
        /// 错误列表
        /// </summary>
        public IList<ErrorModel>? Errors { get; set; } = new List<ErrorModel>();
        /// <summary>
        /// 结果数据
        /// </summary>
        public T? Data { get; set; }
    }
}

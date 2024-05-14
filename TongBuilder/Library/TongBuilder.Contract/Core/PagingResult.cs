namespace TongBuilder.Contract.Core
{
    /// <summary>
    /// 采用泛型分页结果，T为每条数据的类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagingResult<T>
    {
        //构造函数
        public PagingResult(int totalCount, List<T> pageData)
        {
            TotalCount = totalCount;
            PageData = pageData;
        }

        //数据总条数
        public int TotalCount { get; }

        //单页数据列表
        public List<T>? PageData { get; }
    }
}

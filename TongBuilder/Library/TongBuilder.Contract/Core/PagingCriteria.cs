namespace TongBuilder.Contract.Core
{
    public class PagingCriteria
    {
        //是否是点击查询按钮
        public bool IsQuery { get; set; }

        //查询第几页的页码
        public int PageIndex { get; set; } = 1;

        //每页查询多少条数据
        public int PageSize { get; set; } = 5;

        //查询条件参数数据字典
        public Dictionary<string, object> Parameters { get; } = [];

        //排序字段数组，支持多字段排序
        public string[]? OrderBys { get; set; }
    }
}

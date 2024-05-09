namespace TongBuilder.Infrastructure
{
    public interface IUniqueIdGenerator
    {
        string Generate();

        /// <summary>
        /// 生成唯一标识
        /// </summary>
        /// <returns>唯一标识</returns>
        long GenerateLongId();
    }
}

using Microsoft.Extensions.DependencyInjection;
using TongBuilder.Infrastructure.Implementions;

namespace TongBuilder.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加唯一标识产生器
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddUniqueIdGenerator(this IServiceCollection services)
        {
            services.AddSingleton<IUniqueIdGenerator, UniqueIdGenerator>();
            return services;
        }

        /// <summary>
        /// 添加随机数产生器
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRandomGenerator(this IServiceCollection services)
        {
            services.AddSingleton<IRandomGenerator, RandomGenerator>();
            return services;
        }

    }
}

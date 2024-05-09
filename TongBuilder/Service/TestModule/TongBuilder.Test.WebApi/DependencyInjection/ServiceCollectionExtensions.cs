using Microsoft.Extensions.DependencyInjection.Extensions;
using TongBuilder.Contract.Contracts;
using TongBuilder.Application.Identity;

namespace TongBuilder.Test.WebApi.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAspNetCoreIdentity(this IServiceCollection services)
        {
            services.RemoveAll(typeof(ICurrentUserAccessor));
            services.AddSingleton<ICurrentUserAccessor, HttpContextCurrentUserAccessor>();
            services.AddTransient<ICurrentUser, CurrentUser>();
            return services;
        }
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using TongBuilder.Contract.Contracts;

namespace TongBuilder.AuthProxy.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthProxy(this IServiceCollection services, IConfiguration configuration)
        {
            string? etlservice = configuration["Auth:UserLoginServer"] ?? "http://192.168.20.159:8000";
            //ArgumentException.ThrowIfNullOrEmpty(etlservice);
            services.AddHttpClient("AuthProxy", client =>
            {
                client.BaseAddress = new Uri(etlservice);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            });
            services.AddSingleton<IAuthService, AuthServiceProxy>();
            return services;
        }
    }
}

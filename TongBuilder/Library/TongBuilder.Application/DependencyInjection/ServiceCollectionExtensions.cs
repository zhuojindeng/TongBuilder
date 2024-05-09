using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.Http.Headers;
using TongBuilder.Application.Auth;
using TongBuilder.Application.Business;
using TongBuilder.Application.Identity;
using TongBuilder.Contract.Contracts;
using TongBuilder.Contract.Identity;

namespace TongBuilder.Application.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddClientBusiness(this IServiceCollection services, IConfiguration configuration, string? selfurl = null)
        {
            
            //services.AddTransient<JwtHandler>();

            services.RemoveAll(typeof(ICurrentUserAccessor));
            services.AddSingleton<ICurrentUserAccessor, ThreadCurrentUserAccessor>();
            //建议对客户端范围内服务使用 OwningComponentBase 类型来控制服务生存期，应完全不使用可释放的瞬态服务
            services.AddSingleton<ICurrentUser, CurrentUser>();

            string? tongBuilderservice = selfurl;
            if (selfurl == null)
            {
                tongBuilderservice = configuration["TongBuilderService"] ?? "http://localhost:5272/api";//TongBuilder.Test.WebApi
            }

            services.AddHttpClient("TongBuilderProxy", client =>
            {
                client.BaseAddress = new Uri(tongBuilderservice);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            }).AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            //services.AddSingleton<AuthenticationStateProvider, PersistentClientAuthenticationStateProvider>();
            services.AddBlazoredLocalStorageAsSingleton();
            services.AddSingleton<IBizProvider, BizProvider>();
            services.AddSingleton<IWeatherService, WeatherService>();
            services.AddSingleton<IAuthService, AuthService>();            
            return services;
        }        
    }
}

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

            services.AddBlazoredLocalStorageAsSingleton();
            services.AddSingleton<IBizProvider, BizProvider>();
            services.AddSingleton<IWeatherService, WeatherService>();
            services.AddSingleton<IAuthService, AuthService>();            
            return services;
        }

        public static IServiceCollection AddServerBusiness(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<JwtHandler>();
            services.RemoveAll(typeof(ICurrentUserAccessor));
            //必须避免将 IHttpContextAccessor 与交互式呈现一起使用，因为无有效的 HttpContext 可用。
            //HttpContext 只能在常规任务的静态呈现根组件中用作级联参数，对于交互式呈现，值始终是 null。
            services.AddSingleton<ICurrentUserAccessor, HttpContextCurrentUserAccessor>();
            //services.AddSingleton<ICurrentUserAccessor, ThreadCurrentUserAccessor>();
            services.AddTransient<ICurrentUser, CurrentUser>();

            services.AddScoped<AuthenticationStateProvider, PersistingAuthenticationStateProvider>();

            string? tongBuilderservice = configuration["TongBuilderService"] ?? "http://localhost:5272/api";   ////TongBuilder.Test.WebApi         

            services.AddHttpClient("TongBuilderProxy", client =>
            {
                client.BaseAddress = new Uri(tongBuilderservice);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));                
            }).AddHttpMessageHandler<JwtHandler>();

            services.AddBlazoredLocalStorage();
            services.AddScoped<IBizProvider, BizProvider>();
            services.AddScoped<IWeatherService, WeatherService>();
            services.AddScoped<IAuthService, AuthService>();
            
            return services;
        }
               
    }
}

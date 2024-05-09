using Microsoft.Extensions.DependencyInjection;
using McMaster.NETCore.Plugins;
using TongBuilder.Contract.Plugins;
using TongBuilder.Contract.Contracts;
using Microsoft.Extensions.Configuration;
using TongBuilder.Application.Server.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using TongBuilder.Contract.Identity;
using TongBuilder.Application.Server.Identity;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TongBuilder.Application.Server.Business;
using Blazored.LocalStorage;

namespace TongBuilder.Application.Server.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Load a service plugin from plugins directory and invoke the ConfigureService() method on plugin.
        /// This will allow plugin to register the required services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="pluginName"></param>
        /// <param name="sharedTypes"></param>
        /// <returns></returns>
        public static IServiceCollection AddPlugin<T>(this IServiceCollection services, Plugin plugin,
            Action<T, IServiceCollection> configure)
        {
            string pluginsDirectory = Path.Combine(AppContext.BaseDirectory, plugin.Path, plugin.Name);
            if (Directory.Exists(pluginsDirectory))
            {
                var pluginFile = Directory.GetFiles(pluginsDirectory, "*.dll").Where(f => Path.GetFileNameWithoutExtension(f).Equals(plugin.Name)).Single();
                var loader = PluginLoader.CreateFromAssemblyFile(pluginFile, c => { c.PreferSharedTypes = true; });
                foreach (var type in loader.LoadDefaultAssembly().GetTypes()
                    .Where(t => typeof(T).IsAssignableFrom(t) && !t.IsAbstract))
                {
                    var servicePlugin = (T)Activator.CreateInstance(type);
                    configure(servicePlugin, services);
                }
            }
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

            //services.AddScoped<AuthenticationStateProvider, PersistingAuthenticationStateProvider>();
            services.AddBlazoredLocalStorage();
            services.AddScoped<IBizProvider, BizProvider>();
            services.AddScoped<IWeatherService, WeatherService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}

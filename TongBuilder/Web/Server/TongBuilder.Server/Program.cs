using Microsoft.AspNetCore.Components;
using TongBuilder.Server.Components;
using TongBuilder.RazorLib.Services;

namespace TongBuilder.Server
{
    public class Program
    {
        /// <summary>
        /// Blazor Server 托管模型
        /// 这种模式只支持服务端交互,在app设置RenderMode.InteractiveServer共享库才有用，但interactivecounter无法访问
        /// 不设置RenderMode，interactivecounter可用，但共享库失效        
        /// </summary>
        /// <remarks>
        /// 默认情况下，组件使用静态服务器端呈现（静态 SSR）
        /// </remarks>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri(sp.GetService<NavigationManager>().BaseUri)
            });

            builder.Services.AddLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Trace);
                logging.AddDebug();
            });

            builder.Services.AddCommonServices(builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddAdditionalAssemblies(typeof(TongBuilder.RazorLib._Imports).Assembly)
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}

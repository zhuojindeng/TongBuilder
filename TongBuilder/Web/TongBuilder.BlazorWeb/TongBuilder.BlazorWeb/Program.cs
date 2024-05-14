using TongBuilder.BlazorWeb.Components;
using TongBuilder.BlazorWeb.Services;
using TongBuilder.RazorLib.Services;
using TongBuilder.Application.Server.DependencyInjection;
using NLog;
using NLog.Web;
using TongBuilder.Application.Server.Auth;
using TongBuilder.Contract.Contracts;
using TongBuilder.Application.Services;

namespace TongBuilder.BlazorWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLog.LogManager.Setup().LoadConfigurationFromFile("NLog.config").GetCurrentClassLogger();
            logger.Info("Starting up");
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.
                builder.Services.AddRazorComponents()
                    .AddInteractiveServerComponents()
                    .AddInteractiveWebAssemblyComponents();

                builder.Logging.ClearProviders();
                builder.Host.UseNLog();

                builder.Services.AddSingleton<IReadFile, ReadFile>();
                builder.Services.AddServerBusiness(builder.Configuration);
                builder.Services.AddCommonServices(builder.Configuration);

                builder.Services.AddAuthorization();//基于角色的授权和基于Scheme的授权，只是一种语法上的便捷，最终都会生成授权策略
                builder.Services.AddJwtAuthentication(builder.Configuration.GetSection("JwtSettings"));

                builder.Services.AddScoped<IWeatherForecastService, TongBuilder.BlazorWeb.Services.WeatherForecastService>();

                builder.Services.AddHttpContextAccessor();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseWebAssemblyDebugging();
                }
                else
                {
                    app.UseExceptionHandler("/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                app.UseHttpsRedirection();

                app.UseStaticFiles();
                app.UseAntiforgery();

                app.MapRazorComponents<App>()
                    .AddInteractiveServerRenderMode()
                    .AddInteractiveWebAssemblyRenderMode()
                    .AddAdditionalAssemblies(typeof(TongBuilder.RazorLib._Imports).Assembly, typeof(Client._Imports).Assembly);

                app.MapGet("/api/WeatherForecast/GetWeather", async (IWeatherForecastService weatherService) =>
                {
                    return await weatherService.GetForecastAsync(DateTime.Now);
                });
                app.Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                throw;
            }
            finally
            {
                logger.Info("Shut down complete.");
                NLog.LogManager.Shutdown();
            }
        }
    }
}

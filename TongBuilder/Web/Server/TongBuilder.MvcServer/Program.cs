using TongBuilder.MvcServer.Components;
using TongBuilder.MvcServer.Services;
using TongBuilder.RazorLib.Services;
using TongBuilder.Application.DependencyInjection;
using NLog;
using NLog.Web;
using TongBuilder.Application.Server.Auth;

var logger = NLog.LogManager.Setup().LoadConfigurationFromFile("NLog.config").GetCurrentClassLogger();
logger.Info("Starting up");
try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllersWithViews();
    //builder.Services.AddServerSideBlazor(); // 注入服务端Blazor服务
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    /// <summary>
    /// Blazor Server 托管模型
    /// 这种模式只支持服务端交互,在app设置RenderMode.InteractiveServer共享库才有用，但interactivecounter无法访问
    /// 不设置RenderMode，interactivecounter可用，但共享库失效        
    /// </summary>
    /// <remarks>
    /// 默认情况下，组件使用静态服务器端呈现（静态 SSR）
    /// </remarks>
    /// <param name="args"></param>


    //builder.Services.AddScoped(sp => new HttpClient
    //{
    //    BaseAddress = new Uri(sp.GetService<NavigationManager>().BaseUri)
    //});    

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    builder.Services.AddHttpContextAccessor();

    logger.Info($"Registering ServiceBusiness");
    builder.Services.AddServerBusiness(builder.Configuration);
    logger.Info($"Registering CommonServices");
    builder.Services.AddCommonServices(builder.Configuration);
    builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();

    builder.Services.AddAuthorization();//基于角色的授权和基于Scheme的授权，只是一种语法上的便捷，最终都会生成授权策略
    builder.Services.AddJwtAuthentication(builder.Configuration.GetSection("JwtSettings"));


    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.UseAntiforgery();

    //app.MapBlazorHub();
    app.MapRazorComponents<App>() //添加静态服务器端呈现（静态 SSR）    
                    .AddAdditionalAssemblies(typeof(TongBuilder.RazorLib._Imports).Assembly)
                    .AddInteractiveServerRenderMode();//添加到采用交互式服务器端呈现（交互式 SSR）

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    //app.Urls.Add("http://+:5009");
    //app.Urls.Add("http://+:5099");

    app.MapGet("/api/WeatherForecast/GetWeather", async (IWeatherForecastService weatherService, ILogger<Program> logger1) =>
    {
        logger1.LogInformation("Testing logging in GetWeather");
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

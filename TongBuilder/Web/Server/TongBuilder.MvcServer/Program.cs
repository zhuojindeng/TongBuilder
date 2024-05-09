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
    //builder.Services.AddServerSideBlazor(); // ע������Blazor����
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    /// <summary>
    /// Blazor Server �й�ģ��
    /// ����ģʽֻ֧�ַ���˽���,��app����RenderMode.InteractiveServer���������ã���interactivecounter�޷�����
    /// ������RenderMode��interactivecounter���ã��������ʧЧ        
    /// </summary>
    /// <remarks>
    /// Ĭ������£����ʹ�þ�̬�������˳��֣���̬ SSR��
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

    builder.Services.AddAuthorization();//���ڽ�ɫ����Ȩ�ͻ���Scheme����Ȩ��ֻ��һ���﷨�ϵı�ݣ����ն���������Ȩ����
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
    app.MapRazorComponents<App>() //��Ӿ�̬�������˳��֣���̬ SSR��    
                    .AddAdditionalAssemblies(typeof(TongBuilder.RazorLib._Imports).Assembly)
                    .AddInteractiveServerRenderMode();//��ӵ����ý���ʽ�������˳��֣�����ʽ SSR��

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

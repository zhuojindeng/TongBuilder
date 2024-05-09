//此工程作为TongBuilder.Wasm的容器，并为它增加API能力
using NLog;

var logger = NLog.LogManager.Setup().LoadConfigurationFromFile("NLog.config").GetCurrentClassLogger();
logger.Info("Starting up");
try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseWebAssemblyDebugging();
    }

    app.UseHttpsRedirection();

    app.UseBlazorFrameworkFiles();//使 API 能够为 Blazor 应用程序提供服务  
    app.UseStaticFiles();//使静态文件能够由 API 提供服务。

    app.UseAuthorization();

    app.UseRouting();

    app.MapControllers();
    app.MapFallbackToFile("index.html");//如果请求与控制器不匹配，则提供来自 Blazor 项目的 index.html 文件。

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

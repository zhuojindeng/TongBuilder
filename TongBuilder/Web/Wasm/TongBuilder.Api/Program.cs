//�˹�����ΪTongBuilder.Wasm����������Ϊ������API����
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

    app.UseBlazorFrameworkFiles();//ʹ API �ܹ�Ϊ Blazor Ӧ�ó����ṩ����  
    app.UseStaticFiles();//ʹ��̬�ļ��ܹ��� API �ṩ����

    app.UseAuthorization();

    app.UseRouting();

    app.MapControllers();
    app.MapFallbackToFile("index.html");//����������������ƥ�䣬���ṩ���� Blazor ��Ŀ�� index.html �ļ���

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


using AspectCore.Extensions.DependencyInjection;
using NLog;
using NLog.Web;
using TongBuilder.Test.WebApi.DependencyInjection;

namespace TongBuilder.Test.WebApi
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
                builder.AddServiceDefaults();

                // Add services to the container.
                builder.Logging.ClearProviders();
                builder.Host.UseNLog();

                builder.Host.UseServiceProviderFactory(new DynamicProxyServiceProviderFactory());
                builder.Services.AddAspNetCoreIdentity();

                builder.Services.AddCors(options => options.AddPolicy("cors", builder => builder
                           .SetIsOriginAllowed(origin => true)
                           .AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .WithOrigins("*")//"http://localhost:5000", "https://localhost:44365"
                           .AllowCredentials()));

                builder.Services.AddControllers();
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                var app = builder.Build();

                app.MapDefaultEndpoints();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseAuthorization();


                app.MapControllers();

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

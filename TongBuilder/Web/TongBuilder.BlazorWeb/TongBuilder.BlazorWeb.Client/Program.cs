using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TongBuilder.RazorLib.Services;
using TongBuilder.Application.DependencyInjection;

namespace TongBuilder.BlazorWeb.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Trace);
                logging.AddDebug();
            });

            builder.Services.AddClientBusiness(builder.Configuration, builder.HostEnvironment.BaseAddress);
            builder.Services.AddCommonServices(builder.Configuration);
            var host = builder.Build();
            var logger = host.Services.GetRequiredService<ILoggerFactory>()
                .CreateLogger<Program>();

            logger.LogInformation("Logged after the app is built in the Program file.");
            await host.RunAsync();
        }
    }
}

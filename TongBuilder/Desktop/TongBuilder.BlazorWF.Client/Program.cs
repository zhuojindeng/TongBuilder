using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TongBuilder.RazorLib.Services;
using TongBuilder.Application.DependencyInjection;

namespace TongBuilder.BlazorWF.Client
{
    internal static class Program
    {
        internal static ServiceCollection ServiceCollection;
        

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            ServiceCollection = new ServiceCollection();
            ConfigureServices(ServiceCollection);

            var serviceProvider = ServiceCollection.BuildServiceProvider();
            var formMain = serviceProvider.GetRequiredService<MainPage>();
            System.Windows.Forms.Application.Run(formMain);            
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            var builder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("wwwroot/appsettings.json", optional: false, reloadOnChange: true);

            var configuration = builder.Build();
            services.AddSingleton<IConfiguration>(configuration);


            services.AddWindowsFormsBlazorWebView();
#if DEBUG
            services.AddBlazorWebViewDeveloperTools();
#endif          
            services.AddLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Trace);
                logging.AddDebug();
            });
            services.AddClientBusiness(configuration);
            services.AddCommonServices(configuration);
            
            services.AddScoped<MainPage>();

        }
    }
}
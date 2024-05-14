using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Windows;
using TongBuilder.RazorLib.Services;
using TongBuilder.Application.DependencyInjection;
using TongBuilder.Contract.Contracts;
using TongBuilder.Application.Services;

namespace TongBuilder.BlazorWPF.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var exception = e.ExceptionObject as Exception;
                if (exception != null)
                {
                    HandleException(exception);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                //ignore
            }
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            try
            {
                var exception = e.Exception as Exception;
                if (exception != null)
                {
                    HandleException(exception);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                e.SetObserved();
            }
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                HandleException(e.Exception);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                e.Handled = true;
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("wwwroot/appsettings.json", optional: false, reloadOnChange: true);

            var configuration = builder.Build();
            services.AddSingleton<IConfiguration>(configuration);

            services.AddWpfBlazorWebView();
#if DEBUG
            services.AddBlazorWebViewDeveloperTools();
#endif
            services.AddLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Trace);
                logging.AddDebug();
            });

            services.AddSingleton<IReadFile, ReadFile>();
            services.AddClientBusiness(configuration);
            services.AddCommonServices(configuration);   
            services.AddTransient(typeof(MainWindow));

            Resources.Add("services", services.BuildServiceProvider());


        }

        private static void HandleException(Exception ex)
        {
            MessageBox.Show("出错了，请与开发人员联系：" + ex.Message);            
        }
    }

}

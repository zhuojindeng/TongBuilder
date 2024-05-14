using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TongBuilder.RazorLib.Services;
using TongBuilder.Application.DependencyInjection;
using System.Diagnostics;
using TongBuilder.Contract.Contracts;
using TongBuilder.Application.Services;


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
            System.Windows.Forms.Application.ThreadException += Application_ThreadException;
            System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            ServiceCollection = new ServiceCollection();
            ConfigureServices(ServiceCollection);

            var serviceProvider = ServiceCollection.BuildServiceProvider();
            var formMain = serviceProvider.GetRequiredService<MainPage>();
            System.Windows.Forms.Application.Run(formMain);            
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.ExceptionObject;
                string errorMsg = "An application error occurred. Please contact the adminstrator " +
                    "with the following information:\n\n";
                
                if (!EventLog.SourceExists("ThreadException"))
                {
                    EventLog.CreateEventSource("ThreadException", "Application");
                }
                
                EventLog myLog = new EventLog();
                myLog.Source = "ThreadException";
                myLog.WriteEntry(errorMsg + ex.Message + "\n\nStack Trace:\n" + ex.StackTrace);
            }
            catch (Exception exc)
            {
                try
                {
                    MessageBox.Show("Fatal Non-UI Error",
                        "Fatal Non-UI Error. Could not write the error to the event log. Reason: "
                        + exc.Message, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                finally
                {
                    System.Windows.Forms.Application.Exit();
                }
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            DialogResult result = DialogResult.Cancel;
            try
            {
                result = ShowThreadExceptionDialog("Windows Forms Error", e.Exception);
            }
            catch
            {
                try
                {
                    MessageBox.Show("Fatal Windows Forms Error",
                        "Fatal Windows Forms Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop);
                }
                finally
                {
                    System.Windows.Forms.Application.Exit();
                }
            }
            
            if (result == DialogResult.Abort)
                System.Windows.Forms.Application.Exit();
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
            services.AddSingleton<IReadFile, ReadFile>();
            services.AddClientBusiness(configuration);
            services.AddCommonServices(configuration);
            
            services.AddScoped<MainPage>();

        }

        private static DialogResult ShowThreadExceptionDialog(string title, Exception e)
        {
            string errorMsg = "An application error occurred. Please contact the adminstrator " +
                "with the following information:\n\n";
            errorMsg = errorMsg + e.Message + "\n\nStack Trace:\n" + e.StackTrace;
            return MessageBox.Show(errorMsg, title, MessageBoxButtons.AbortRetryIgnore,
                MessageBoxIcon.Stop);
        }
    }
}
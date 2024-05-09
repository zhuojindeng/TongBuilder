using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using System.Reflection;
using TongBuilder.RazorLib.Services;
using TongBuilder.Application.DependencyInjection;

namespace TongBuilder.BlazorMaui.Client
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            //var logger = LogManager.Setup().RegisterMauiLog()
            //        .LoadConfigurationFromAssemblyResource(typeof(App).Assembly).GetCurrentClassLogger();

            var logger = NLog.LogManager.Setup().RegisterMauiLog()
                 .LoadConfiguration(c => c.ForLogger().FilterMinLevel(NLog.LogLevel.Debug).WriteToMauiLog())
                 .GetCurrentClassLogger();
            logger.Info("Starting up");
            try
            {
                var builder = MauiApp.CreateBuilder();
                builder.Logging.ClearProviders();
                builder.Logging.AddNLog();

                builder
                    .UseMauiApp<App>()
                    .ConfigureFonts(fonts =>
                    {
                        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    });

                builder.Services.AddMauiBlazorWebView();

#if DEBUG
                builder.Services.AddBlazorWebViewDeveloperTools();
                builder.Logging.AddDebug();
#endif

                

                //1：Only for windows
                //var config = new ConfigurationBuilder().AddJsonFile("wwwroot/appsettings.json").Build();

                //2：           
                //var assembly = Assembly.GetExecutingAssembly();
                //var stream = assembly.GetManifestResourceStream("AIStudio.BlazorMaui.Client.wwwroot.appsettings.json");
                //string text = "";
                //using (var reader = new System.IO.StreamReader(stream))
                //{
                //    text = reader.ReadToEnd();
                //}

                //string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "wwwroot/appsettings.json");
                //if (DirFileHelper.IsExistFile(path))
                //{
                //    var currentFileContent = DirFileHelper.ReadFile(path);
                //    var isSameContent = currentFileContent.ToMd5() == text.ToMd5();
                //    if (!isSameContent)
                //    {
                //        DirFileHelper.CreateFile(path, text);
                //    }
                //}
                //else
                //{
                //    DirFileHelper.CreateFile(path, text);

                //}
                //var config = new ConfigurationBuilder().AddJsonFile(path).Build();


                var assembly = Assembly.GetExecutingAssembly();
                var stream = assembly.GetManifestResourceStream("TongBuilder.BlazorMaui.Client.wwwroot.appsettings.json");
                var config = new ConfigurationBuilder().AddJsonStream(stream).Build();

                builder.Configuration.AddConfiguration(config);
                builder.Services.AddClientBusiness(builder.Configuration);
                builder.Services.AddCommonServices(builder.Configuration);

                builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);

                return builder.Build();
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message, ex);
                throw;
            }
            //finally
            //{
            //    logger.Info("Shut down complete.");
            //    NLog.LogManager.Shutdown();
            //}
            
        }
    }
}

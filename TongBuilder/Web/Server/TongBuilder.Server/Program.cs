using Microsoft.AspNetCore.Components;
using TongBuilder.Server.Components;
using TongBuilder.RazorLib.Services;

namespace TongBuilder.Server
{
    public class Program
    {
        /// <summary>
        /// Blazor Server �й�ģ��
        /// ����ģʽֻ֧�ַ���˽���,��app����RenderMode.InteractiveServer���������ã���interactivecounter�޷�����
        /// ������RenderMode��interactivecounter���ã��������ʧЧ        
        /// </summary>
        /// <remarks>
        /// Ĭ������£����ʹ�þ�̬�������˳��֣���̬ SSR��
        /// </remarks>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri(sp.GetService<NavigationManager>().BaseUri)
            });

            builder.Services.AddLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Trace);
                logging.AddDebug();
            });

            builder.Services.AddCommonServices(builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddAdditionalAssemblies(typeof(TongBuilder.RazorLib._Imports).Assembly)
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}

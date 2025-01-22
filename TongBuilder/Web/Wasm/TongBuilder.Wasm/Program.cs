using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TongBuilder.RazorLib;
using TongBuilder.RazorLib.Services;
using TongBuilder.Application.DependencyInjection;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using TongBuilder.Contract.Contracts;
using TongBuilder.Application.Services;

namespace TongBuilder.Wasm
{
    public class Program
    {
        /// <summary>
        /// Blazor WebAssembly �й�ģ��,���� Blazor WebAssembly Ӧ��
        /// �ɲ���Ϊ��̬վ��Ķ����ͻ��� Web Ӧ��
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<Routes>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            //https://learn.microsoft.com/zh-cn/aspnet/core/blazor/security/webassembly/additional-scenarios?view=aspnetcore-8.0
            //builder.Services.AddHttpClient("ServerAPI")
            //                .ConfigureHttpClient(client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
            //                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();//��Ԥ��������Ӧ�û�ַ��Ϊ��Ȩ�� URL�� �������� URI ��Ӧ�õĻ� URI ��ʱ���Ż���ӷ������ơ�

            //// Supply HttpClient instances that include access tokens when making requests to the server project.
            //builder.Services.AddScoped(provider =>
            //{
            //    var factory = provider.GetRequiredService<IHttpClientFactory>();
            //    return factory.CreateClient("ServerAPI");
            //});

            builder.Services.AddLogging(logging =>
            {
                logging.SetMinimumLevel(LogLevel.Trace);
                logging.AddDebug();
            });
            builder.Services.AddSingleton<IReadFile, ReadFile>();
            builder.Services.AddClientBusiness(builder.Configuration, builder.HostEnvironment.BaseAddress);//
            builder.Services.AddCommonServices(builder.Configuration);

            var host = builder.Build();

            var logger = host.Services.GetRequiredService<ILoggerFactory>()
                .CreateLogger<Program>();

            logger.LogInformation("Logged after the app is built in the Program file.");


            await host.RunAsync();
        }
    }
}

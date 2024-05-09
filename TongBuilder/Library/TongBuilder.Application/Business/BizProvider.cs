using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using TongBuilder.Contract.Contracts;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TongBuilder.Application.Business
{
    public class BizProvider: IBizProvider, IDisposable
    {
        private readonly HttpClient _client;
        //在预呈现期间，localStorage不可用，若要禁用预呈现，
        //请通过在应用组件层次结构中的最高级别组件（不是根组件，例如 App 组件）处将 prerender 参数设置为 false 来指示呈现模式。
        //<Routes @rendermode="new InteractiveServerRenderMode(prerender: false)" />
        //<HeadOutlet @rendermode="new InteractiveServerRenderMode(prerender: false)" />
        private readonly ILocalStorageService _storage;
        private readonly NavigationManager _navigation;
        private readonly ILogger<BizProvider> _logger = null!;
        private JsonSerializerOptions _options;

        private const string TOKEN = "Token";

        public BizProvider(IHttpClientFactory httpClientFactory, ILocalStorageService storage, 
            NavigationManager navigation, ILogger<BizProvider> logger)
        {
            _logger = logger;                    
            _storage = storage;
            _navigation = navigation;
            _client = httpClientFactory.CreateClient("TongBuilderService");
            _options = GetSerializerOptions();           
            TimeOut = TimeSpan.FromSeconds(20);
        }
        
        public TimeSpan TimeOut { get; set; }

        public async Task<Dictionary<string, string>> GetHeader()
        {
            var token = await _storage.GetItemAsStringAsync(TOKEN);

            Dictionary<string, string> header = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(token))
            {
                header.Add("Authorization", string.Format("Bearer {0}", token));
            }
            else
            {
                throw new Exception("No Token");
            }
            return header;
        }

        private JsonSerializerOptions GetSerializerOptions()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());
            //options.Converters.Add(new TimeSpanConverter());
            //options.Converters.Add(new DateTimeConverter("yyyy-MM-dd HH:mm:ss.fff"));
            return options;
        }

        public void Dispose() => _client?.Dispose();
    }
}

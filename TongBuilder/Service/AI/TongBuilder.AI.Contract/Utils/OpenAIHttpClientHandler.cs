using TongBuilder.AI.Contract.Options;

namespace TongBuilder.AI.Contract.Utils
{
    public class OpenAIHttpClientHandler : HttpClientHandler
    {
        private KernelSetting _kernelSetting;

        public OpenAIHttpClientHandler(KernelSetting setting)
        {
            this._kernelSetting = setting;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri?.LocalPath == "/v1/chat/completions")
            {
                UriBuilder uriBuilder = new UriBuilder(request.RequestUri)
                {
                    Scheme = this._kernelSetting.Scheme,
                    Host = this._kernelSetting.Host,
                    Port = this._kernelSetting.Port
                };
                request.RequestUri = uriBuilder.Uri;
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}

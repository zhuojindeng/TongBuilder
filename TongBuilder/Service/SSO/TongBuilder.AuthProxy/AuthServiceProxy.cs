using Microsoft.Extensions.Logging;
using SiweiSoft.Application;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using TongBuilder.Contract.Contracts;
using TongBuilder.Contract.Models;
using SiweiSoft.Serialization.Json.Converters;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace TongBuilder.AuthProxy
{
    public class AuthServiceProxy : IAuthService
    {       
        public event Action<ClaimsPrincipal>? UserChanged;

        private ClaimsPrincipal? currentUser;
        private readonly Random _random = new Random();
        private readonly HttpClient _client;
        private JsonSerializerOptions _options;
        private readonly ILogger<AuthServiceProxy> _logger = null!;

        public AuthServiceProxy(IHttpClientFactory httpClientFactory, ILogger<AuthServiceProxy> logger)
        {
            _logger = logger;
            _client = httpClientFactory.CreateClient("AuthProxy");            
            _options = GetSerializerOptions();
        }

        private JsonSerializerOptions GetSerializerOptions()
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());
            options.Converters.Add(new TimeSpanConverter());
            options.Converters.Add(new DateTimeConverter("yyyy-MM-dd HH:mm:ss.fff"));
            return options;
        }

        public ClaimsPrincipal CurrentUser
        {
            get { return currentUser ?? new(); }
            set
            {
                currentUser = value;

                if (UserChanged is not null)
                {
                    UserChanged(currentUser);
                }
            }
        }

        public Task<ICurrentUser> GetCurrentUser()
        {
            throw new NotImplementedException();
        }

        public Task<UserLoginResultModel> Login(UserLoginModel userLoginModel)
        {
            throw new NotImplementedException();
        }

        public Task Logout()
        {
            throw new NotImplementedException();
        }

        public async Task<OperationResult<ExternalUser>> GetExternalUserAsync(string loginProvider, string providerKey, CancellationToken cancellationToken = default)
        {
            //测试token，待用户管理服务参数确定后改为根据用户openid和issuer服务器生成            
            var failReason = "";
            try
            {
                var response = await _client.GetAsync(
               $"api/externaluser/GetByProviderKey?LoginProvider={loginProvider}&ProviderKey={providerKey}", cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    var ret = await response.Content.ReadFromJsonAsync<OperationResult<ExternalUser>>(_options);
                    return ret;
                }
                failReason = response.ReasonPhrase;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetExternalUserAsync: {Error}", ex);
                failReason = ex.Message;
            }
            return OperationResult<ExternalUser>.Failed("-1", $"GetExternalUserAsync 失败:{failReason}");
        }

        public async Task<OperationResult<IList<TenantDto>>> GetTenantsByUserIdAsync(string openId, CancellationToken cancellationToken = default)
        {
            var failReason = "";
            try
            {
                var response = await _client.GetAsync(
               $"api/user/tenants?openid={openId}", cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    var ret = await response.Content.ReadFromJsonAsync<OperationResult<IList<TenantDto>>>(_options);
                    return ret;
                }
                failReason = response.ReasonPhrase;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetTenantsByUserIdAsync: {Error}", ex);
                failReason = ex.Message;
            }
            return OperationResult<IList<TenantDto>>.Failed("-1", $"GetTenantsByUserIdAsync 失败:{failReason}");
        }

        public void SetToken(string token)
        {            
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<OperationResult<UserInfo>> GetUserInfoAsync(CancellationToken cancellationToken)
        {
            var failReason = "";
            try
            {
                var response = await _client.GetAsync(
               $"api/user/info", cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    var ret = await response.Content.ReadFromJsonAsync<OperationResult<UserInfo>>(_options);
                    return ret;
                }
                failReason = response.ReasonPhrase;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetUserInfoAsync: {Error}", ex);
                failReason = ex.Message;
            }
            return OperationResult<UserInfo>.Failed("-1", $"GetUserInfoAsync 失败:{failReason}");
        }

        public Task<string> GetCaptchaAsync(string modile)
        {
            var captcha = _random.Next(0, 9999).ToString().PadLeft(4, '0');
            return Task.FromResult(captcha);
        }
    }
}

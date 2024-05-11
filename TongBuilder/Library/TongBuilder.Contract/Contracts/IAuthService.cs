using SiweiSoft.Application;
using System.Security.Claims;
using TongBuilder.Contract.Models;

namespace TongBuilder.Contract.Contracts
{
    public interface IAuthService
    {
        event Action<ClaimsPrincipal>? UserChanged;

        ClaimsPrincipal CurrentUser { get; }

        Task<ICurrentUser> GetCurrentUser();

        Task<UserLoginResultModel> Login(UserLoginModel userLoginModel);

        Task Logout();

        void SetToken(string token);

        Task<string> GetCaptchaAsync(string modile);

        Task<OperationResult<UserInfo>> GetUserInfoAsync(CancellationToken cancellationToken = default);

        Task<OperationResult<ExternalUser>> GetExternalUserAsync(string loginProvider, string providerKey, CancellationToken cancellationToken = default);

        Task<OperationResult<IList<TenantDto>>> GetTenantsByUserIdAsync(string openId, CancellationToken cancellationToken = default);
    }
}

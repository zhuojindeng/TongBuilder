using SiweiSoft.Application;
using System.Security.Claims;
using TongBuilder.Contract.Contracts;
using TongBuilder.Contract.Models;

namespace TongBuilder.Application.Server.Business
{
    public class AuthService : IAuthService
    {
        public event Action<ClaimsPrincipal>? UserChanged;

        private ClaimsPrincipal? currentUser;

        private IBizProvider _iBizProvider;

        private ICurrentUser _iCurrentUser;

        public AuthService(IBizProvider iBizProvider, ICurrentUser iCurrentUser)
        {
            _iBizProvider = iBizProvider;
            _iCurrentUser = iCurrentUser;
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

        public async Task<UserLoginResultModel> Login(UserLoginModel userLoginModel)
        {
            // login to get UserLoginResultModel
            // to get ClaimsIdentity by UserLoginResultModel
            var claims = new Claim[] { new Claim(ClaimTypes.Name, userLoginModel.UserName), new Claim(ClaimTypes.Role, "Admin") };
            //用户标识
            var identity = new ClaimsIdentity(userLoginModel.LoginMode);
            identity.AddClaims(claims);
            // set CurrentUser
            CurrentUser = new ClaimsPrincipal(identity);
            using (_iCurrentUser.Change(CurrentUser))
            {

            }
            return new UserLoginResultModel() { Succeeded = true };
        }

        public async Task Logout()
        {
            
        }

        public Task<OperationResult<ExternalUser>> GetExternalUserAsync(string loginProvider, string providerKey, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void SetToken(string token)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<IList<TenantDto>>> GetTenantsByUserIdAsync(string openId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult<UserInfo>> GetUserInfoAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

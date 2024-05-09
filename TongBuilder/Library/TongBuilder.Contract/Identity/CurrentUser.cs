using System.Security.Claims;
using TongBuilder.Contract.Consts;
using TongBuilder.Contract.Contracts;
using TongBuilder.Contract.Enums;
using TongBuilder.Contract.Extensions;

namespace TongBuilder.Contract.Identity
{
    public class CurrentUser : ICurrentUser
    {
        private readonly ICurrentUserAccessor _currentUserAccessor;        

        public ClaimsPrincipal? Principal => _currentUserAccessor.Principal;

        public bool IsAuthenticated => Principal != null && Principal.Identity != null && Principal.Identity.IsAuthenticated;

        public string? OpenId => this.FindClaimValue(CustomClaimTypes.Subject);

        public IdentityUserType UserType
        {
            get
            {
                var type = this.FindClaimValue(CustomClaimTypes.UserType);
                if (Enum.TryParse<IdentityUserType>(type, out var userType))
                {
                    return userType;
                }
                return IdentityUserType.Normal;
            }
        }

        public long? UserId => this.FindClaimValue<long>(CustomClaimTypes.NameIdentifier);

        public string? UserName => this.FindClaimValue(CustomClaimTypes.Code);

        public string? NickName => this.FindClaimValue(CustomClaimTypes.Name);

        public long? AccountId => this.FindClaimValue<long>(CustomClaimTypes.AccountIdentifier);

        public string? AccountCode => this.FindClaimValue(CustomClaimTypes.AccountCode);

        public string? AccountName => this.FindClaimValue(CustomClaimTypes.AccountName);

        public AccountType AccountType
        {
            get
            {
                var type = this.FindClaimValue(CustomClaimTypes.AccountType);
                if (Enum.TryParse<AccountType>(type, out var accountType))
                {
                    return accountType;
                }
                return AccountType.None;
            }
        }

        public IList<string> Roles
        {
            get
            {
                var role = this.FindClaimValue(CustomClaimTypes.Role);
                if (role != null) return role.Split(",");
                return new List<string>();
            }
        }

        public CurrentUser(ICurrentUserAccessor currentUserAccessor)
        {
            _currentUserAccessor = currentUserAccessor;
        }

        public Claim? FindClaim(string claimType)
        {
            return Principal?.Claims.FirstOrDefault(t => t.Type == claimType);
        }

        public IDisposable Change(ClaimsPrincipal principal)
        {
            return _currentUserAccessor.Change(principal);
        }
    }
}

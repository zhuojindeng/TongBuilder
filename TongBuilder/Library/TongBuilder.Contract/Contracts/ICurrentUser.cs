using System.Security.Claims;
using TongBuilder.Contract.Enums;

namespace TongBuilder.Contract.Contracts
{
    public interface ICurrentUser
    {
        ClaimsPrincipal? Principal { get; }

        bool IsAuthenticated { get; }

        string? OpenId { get; }

        IdentityUserType UserType { get; }

        long? UserId { get; }

        string? UserName { get; }

        string? NickName { get; }

        long? AccountId { get; }

        string? AccountCode { get; }

        string? AccountName { get; }

        AccountType AccountType { get; }

        IList<string> Roles { get; }

        Claim? FindClaim(string claimType);

        IDisposable Change(ClaimsPrincipal principal);
    }
}

using System.Security.Claims;

namespace TongBuilder.Contract.Contracts
{
    public interface ICurrentUserAccessor
    {
        ClaimsPrincipal? Principal { get; }

        IDisposable Change(ClaimsPrincipal principal);
    }
}

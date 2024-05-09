using System.Security.Claims;
using TongBuilder.Contract.Identity;

namespace TongBuilder.Application.Identity
{
    public class ThreadCurrentUserAccessor : CurrentUserAccessorBase
    {
        protected override ClaimsPrincipal? GetClaimsPrincipal()
        {
            return Thread.CurrentPrincipal as ClaimsPrincipal;
        }
    }
}

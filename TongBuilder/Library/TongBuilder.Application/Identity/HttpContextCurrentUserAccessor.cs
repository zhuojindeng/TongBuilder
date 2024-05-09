using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace TongBuilder.Application.Identity
{
    public class HttpContextCurrentUserAccessor : CurrentUserAccessorBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextCurrentUserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override ClaimsPrincipal? GetClaimsPrincipal()
        {
            return _httpContextAccessor.HttpContext?.User;
        }
    }
}

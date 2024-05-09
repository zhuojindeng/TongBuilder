using System.Security.Claims;
using TongBuilder.Contract.Contracts;
using TongBuilder.Contract.Core;

namespace TongBuilder.Contract.Identity
{
    public abstract class CurrentUserAccessorBase : ICurrentUserAccessor
    {
        private readonly AsyncLocal<ClaimsPrincipalHolder> _currentPrincipal = new AsyncLocal<ClaimsPrincipalHolder>();

        public ClaimsPrincipal? Principal => _currentPrincipal.Value?.Principal ?? GetClaimsPrincipal();

        protected abstract ClaimsPrincipal? GetClaimsPrincipal();

        public virtual IDisposable Change(ClaimsPrincipal principal)
        {
            return SetCurrent(principal);
        }

        private IDisposable SetCurrent(ClaimsPrincipal principal)
        {
            var parent = Principal;
            var holder = _currentPrincipal.Value;
            if (holder != null)
            {
                holder.Principal = null;
            }
            _currentPrincipal.Value = new ClaimsPrincipalHolder() { Principal = principal };
            return new DisposableAction(() =>
            {
                _currentPrincipal.Value = new ClaimsPrincipalHolder() { Principal = parent };
            });
        }

        private class ClaimsPrincipalHolder
        {
            public ClaimsPrincipal? Principal { get; set; }
        }
    }
}

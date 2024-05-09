using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using System.Security.Claims;
using TongBuilder.Contract.Contracts;

namespace TongBuilder.Application.Auth
{
    /// <summary>
    /// This is a client-side AuthenticationStateProvider that determines the user's authentication state by
    /// looking for data persisted in the page when it was rendered on the server. This authentication state will
    /// be fixed for the lifetime of the WebAssembly application. So, if the user needs to log in or out, a full
    /// page reload is required.
    /// </summary>
    internal class PersistentClientAuthenticationStateProvider : AuthenticationStateProvider
    {
        private static readonly Task<AuthenticationState> defaultUnauthenticatedTask =
            Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

        private readonly Task<AuthenticationState> authenticationStateTask = defaultUnauthenticatedTask;

        public PersistentClientAuthenticationStateProvider(PersistentComponentState state)
        {
            if (!state.TryTakeFromJson<ICurrentUser>(nameof(ICurrentUser), out var userInfo) || userInfo is null)
            {
                return;
            }

            authenticationStateTask = Task.FromResult(new AuthenticationState(userInfo.Principal));
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync() => authenticationStateTask;
    }
}

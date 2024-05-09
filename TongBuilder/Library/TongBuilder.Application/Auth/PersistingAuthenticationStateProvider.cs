using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using TongBuilder.Contract.Contracts;
using System.Diagnostics;

namespace TongBuilder.Application.Auth
{
    /// <summary>
    /// This is a server-side AuthenticationStateProvider that uses PersistentComponentState to flow the
    /// authentication state to the client which is then fixed for the lifetime of the WebAssembly application.
    /// 为自定义 AuthenticationStateProvider 实现 IHostEnvironmentAuthenticationStateProvider 以支持预呈现
    /// </summary>
    internal sealed class PersistingAuthenticationStateProvider : AuthenticationStateProvider, IHostEnvironmentAuthenticationStateProvider, IDisposable
    {
        private readonly PersistentComponentState persistentComponentState;
        private readonly PersistingComponentStateSubscription subscription;
        private Task<AuthenticationState>? authenticationStateTask;
        private readonly ICurrentUser _user;

        public PersistingAuthenticationStateProvider(PersistentComponentState state, ICurrentUser user)
        {
            persistentComponentState = state;
            AuthenticationStateChanged += OnAuthenticationStateChanged;
            subscription = state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
            _user = user;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync() => authenticationStateTask ??
                throw new InvalidOperationException($"Do not call {nameof(GetAuthenticationStateAsync)} outside of the DI scope for a Razor component. Typically, this means you can call it only within a Razor component or inside another DI service that is resolved for a Razor component.");

        public void SetAuthenticationState(Task<AuthenticationState> task)
        {
            authenticationStateTask = task;
        }

        private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
        {
            authenticationStateTask = task;
        }

        private async Task OnPersistingAsync()
        {
            if (authenticationStateTask is null)
            {
                throw new UnreachableException($"Authentication state not set in {nameof(OnPersistingAsync)}().");
            }
            var authenticationState = await GetAuthenticationStateAsync();
            var principal = authenticationState.User;

            if (principal.Identity?.IsAuthenticated == true)
            {
                persistentComponentState.PersistAsJson(nameof(ICurrentUser), _user);
            }
        }

        public void Dispose()
        {
            subscription.Dispose();
            AuthenticationStateChanged -= OnAuthenticationStateChanged;
        }
    }

}

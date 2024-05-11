using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Security.Claims;
using TongBuilder.Contract.Contracts;
using TongBuilder.Contract.Models;

namespace TongBuilder.RazorLib.Services.Auth
{
    public class TongAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly Random _random = new Random();
        private readonly IAuthService _authService;

        private AuthenticationState _currentUser;

        public TongAuthenticationStateProvider(IAuthService authService)
        {
            _authService = authService;
            _currentUser = new AuthenticationState(authService.CurrentUser);
            //从 BlazorWebView 外发出身份验证更新信号,只适用于client(authService为单例)
            _authService.UserChanged += (newUser) =>
            {
                _currentUser = new AuthenticationState(newUser);
                NotifyAuthenticationStateChanged(Task.FromResult(_currentUser));
            };
        }
        public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
            Task.FromResult(_currentUser);

        private async Task<ICurrentUser> GetCurrentUser()
        {
            return await _authService.GetCurrentUser();
        }

        public async Task Logout()
        {
            await _authService.Logout();  
        }

        public async Task<UserLoginResultModel> Login(UserLoginModel userLoginModel)
        {
            var response = await _authService.Login(userLoginModel);           
            return response;
        }

        public void Refresh(string token)
        {
            if (string.IsNullOrEmpty(token)) return;
            
        }

        public async Task<string> GetCaptchaAsync(string modile)
        {
            var captcha = _random.Next(0, 9999).ToString().PadLeft(4, '0');
            return captcha;
        }
    }
}

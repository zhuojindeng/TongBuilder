using AntDesign;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using TongBuilder.Contract.Models;
using TongBuilder.RazorLib.Models;
using TongBuilder.RazorLib.Services.Auth;

namespace TongBuilder.RazorLib.Components.Pages.User.Login
{
    public partial class Login
    {
        private readonly UserLoginModel _model = new UserLoginModel();
        bool loading = false;

        [Inject] public NavigationManager NavigationManager { get; set; }

        [Inject] public MessageService Message { get; set; }

        /// <summary>
        /// 现期间，localStorage 或 sessionStorage 不可用。 如果组件尝试与存储进行交互，
        /// 则会生成错误，说明由于正在预呈现组件，无法发起 JavaScript 互操作调用。
        /// 解决此错误的一种方法是禁用预呈现(<Routes @rendermode="new InteractiveServerRenderMode(prerender: false)" />)。
        /// (<HeadOutlet @rendermode="new InteractiveServerRenderMode(prerender: false)" />)如果应用大量使用基于浏览器的存储，
        /// 则这通常是最佳选择。 预呈现会增加复杂性，且不会给应用带来好处，
        /// 因为在 localStorage 或 sessionStorage 可用之前，应用无法预呈现任何有用的内容。
        /// https://learn.microsoft.com/zh-cn/aspnet/core/blazor/state-management?pivots=server&view=aspnetcore-8.0
        /// </summary>
        [Inject]
        ILocalStorageService StorageService { get; set; }

        [Inject]
        public AuthenticationStateProvider AuthStateProvider { get; set; }

        public async void HandleSubmit()
        {
            try
            {
                loading = true;
                var password = _model.Password;
               
                var result = await ((TongAuthenticationStateProvider)AuthStateProvider).Login(_model);
                if (result.Succeeded)
                {                    
                    NavigationManager.NavigateTo("/");
                    await Message.Success("登录成功");
                }
                else
                {
                    loading = false;
                    await Message.Warning(result.ErrorMsg);
                }
            }
            catch (System.Exception ex)
            {
                await Message.Error(ex.Message);
            }
            finally
            {

            }
        }
    }
}

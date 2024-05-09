using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using TongBuilder.Contract.Models;
using TongBuilder.SSOServer.Constants;
using TongBuilder.SSOServer.Models;
using TongBuilder.SSOServer.Services;
using TongBuilder.SSOServer.ViewModels;

namespace TongBuilder.SSOServer.Controllers
{
    /// <summary>
    /// 需要将登录界面和用户维护界面独立出来成为独立的Blazor库，使其可以嵌入各种端。
    /// 可以在sso服务器提供所有端的统一界面（用blazor服务端呈现模式）。参考Pixel.Identity.UI.Client（此库仅仅是webassembly），
    /// 将其并入TongBuilder.RazorLib(此库是blazor服务端和客户端的通用库，是否需要专门设计一个blazor webassembly库？)。
    /// 如果用户的注册、登录在专门的服务器，SSO服务器只是转接，相关的界面也可以在用户服务器上
    /// </summary>
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string? _localhost;//记录从配置中获取的本地地址
        private readonly string? _externalhost;//从配置中读取外网地址，以供第三方使用
        private readonly string? _publicKey;//记录从配置中获取的页面密码加密公钥
        private readonly string? _dingtalkAppId;
        private readonly string? _wechatAppId;
        private readonly string? _registerPage;
        private readonly UserLoginService _userLogin;

        public AccountController(IConfiguration configuration, UserLoginService userLogin) 
        {
            _configuration = configuration;
            _localhost = _configuration.GetValue<string>("Auth:Issuer");
            _externalhost = _configuration.GetValue<string>("Auth:ExternalHost");
            _publicKey = Keys.PagePublicKey;
            _dingtalkAppId = _configuration.GetValue<string>("Auth:ThirdParty:DingtalkAppId");
            _wechatAppId = _configuration.GetValue<string>("Auth:ThirdParty:WechatAppId");
            _registerPage = _configuration.GetValue<string>("Auth:ThirdParty:RegisterPage");
            _userLogin = userLogin;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get登录
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = "/Home/Index")//默认值待定，但是不能为null
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Localhost"] = _localhost;
            ViewData["Externalhost"] = _externalhost;
            ViewData["PublicKey"] = _publicKey;
            ViewData["RegisterPage"] = _registerPage;
            return View();
        }

        /// <summary>
        /// Post登录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)//where you check the credentials against your database.
            {
                //登录验证
                var result = await _userLogin.CheckAccountAsync(model);

                if (!result.Succeeded)
                {
                    var errorMessage = string.Join(";", result.Errors.Select(e => e.Description).ToList());

                    ModelState.AddModelError(string.Empty, string.IsNullOrWhiteSpace(errorMessage) ? "短信验证发生了异常" : errorMessage);
                    ViewData["ReturnUrl"] = model.ReturnUrl;
                    ViewData["Localhost"] = _localhost;
                    ViewData["Externalhost"] = _externalhost;
                    ViewData["PublicKey"] = _publicKey;
                    ViewData["LoginType"] = model.LoginType;
                    ViewData["Username"] = model.Username;
                    ViewData["RegisterPage"] = _registerPage;
                    return View(model);
                }

                //跳转到租户选择页面
                result.Tenants.Add(new TenantModel() { Code = "", Name = "以游客身份登录", IsDefault = false });
                ViewData["ThirdPartyLogin"] = false;
                return View("TenantSelection", new TenantViewModel() { TenantList = result.Tenants });
            }

            ViewData["ReturnUrl"] = model.ReturnUrl;
            ViewData["Localhost"] = _localhost;
            ViewData["Externalhost"] = _externalhost;
            ViewData["PublicKey"] = _publicKey;
            ViewData["RegisterPage"] = _registerPage;
            return View(model);
        }
    }
}

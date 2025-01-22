using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using OpenIddict.Abstractions;
using TongBuilder.SSOServer.ViewModels;
using static OpenIddict.Abstractions.OpenIddictConstants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TongBuilder.Infrastructure;
using System.Globalization;
using Microsoft.Extensions.Options;
using TongBuilder.SSOServer.Options;
using MongoDB.Driver;
using TongBuilder.SSOServer.Models;
using TongBuilder.SSOServer.Services;
using TongBuilder.SSOServer.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Immutable;
using Microsoft.AspNetCore.Identity;
using TongBuilder.Contract.Models;

namespace TongBuilder.SSOServer.Controllers
{
    /// <summary>
    /// Controller for handling OpenId protocol using OpenIdDict.
    /// It provides end points for authentication, tokens, sign out , etc.
    /// The Authorization Server will be responsible for authenticating Resource Owner, 
    /// verifying consent from Resource Owner, and issuing the tokens to the Clients
    /// 将sso服务集成到总线，用户管理服务也要集成进总线（用户管理服务对应Microsoft.identity)
    /// 甚至systemMan也要集成进总线
    /// </summary>
    public class AuthorizationController : Controller
    {
        private readonly IOpenIddictApplicationManager _applicationManager;
        private readonly IOpenIddictAuthorizationManager _authorizationManager;
        private readonly IOpenIddictScopeManager _scopeManager;
        private readonly IOpenIddictTokenManager _tokenManager;
        private readonly UserLoginService _userLogin;
        private readonly IUniqueIdGenerator _idGenerator;
        private readonly IRandomGenerator _randomGenerator;
        private readonly IOptions<AuthOptions> _options;
        private readonly IConfiguration _configuration;
        private List<string>? _independentLogoutAppList;

        public AuthorizationController(
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictAuthorizationManager authorizationManager,
        IOpenIddictScopeManager scopeManager, IConfiguration configuration,
        IOpenIddictTokenManager tokenManager, UserLoginService userLogin,
        IUniqueIdGenerator idGenerator, IRandomGenerator randomGenerator,
        IOptions<AuthOptions> options)
        {
            _applicationManager = applicationManager;
            _authorizationManager = authorizationManager;
            _scopeManager = scopeManager;           
            _tokenManager = tokenManager;
            _userLogin = userLogin;
            _idGenerator = idGenerator;
            _randomGenerator = randomGenerator;
            _options = options;
            _configuration = configuration;
        }

        /// <summary>
        /// 独立登出应用ID配置，这些应用不参与统一登出
        /// </summary>
        public List<string> IndependentLogoutAppList
        {
            get
            {
                if (_independentLogoutAppList == null && _configuration != null)
                {
                    _independentLogoutAppList = _configuration.GetValue<string>("IndependentLogoutApps").Split(",").ToList();
                }
                return _independentLogoutAppList;
            }
        }
        #region Authorization code, implicit and hybrid flows
        /// <summary>
        /// 客户端发起登录权限校验（ for AuthorizationCodeFlow）,
        /// 返回authorization code which the client can exchange for an access token by calling the token endpoint.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="Exception"></exception>
        [HttpGet("~/connect/authorize")]
        [HttpPost("~/connect/authorize")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Authorize()
        {
            //Remember that we already implemented authentication in our project. So, in the authorize method we just determine if the user is already logged in,
            //if not, we redirect the user to the login page.
            var request = HttpContext.GetOpenIddictServerRequest() ??
            throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            // If prompt=login was specified by the client application,
            // immediately return the user agent to the login page.
            if (request.HasPromptValue("Prompts.Login"))
            {
                // To avoid endless login -> authorization redirects, the prompt=login flag
                // is removed from the authorization request payload before redirecting the user.
                var prompt = string.Join(" ", request.GetPromptValues().Remove("Prompts.Login"));
                var parameters = Request.HasFormContentType ?
                    Request.Form.Where(parameter => parameter.Key != Parameters.Prompt).ToList() :
                    Request.Query.Where(parameter => parameter.Key != Parameters.Prompt).ToList();

                parameters.Add(KeyValuePair.Create(Parameters.Prompt, new StringValues(prompt)));
                //返回一个需要认证的标识来提示用户登录，通常会返回一个 401 状态码
                return Challenge(
                    authenticationSchemes: JwtBearerDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties
                    {
                        RedirectUri = Request.PathBase + Request.Path + QueryString.Create(parameters)
                    });
            }

            // Retrieve the user principal stored in the Jwt.            
            var result = await HttpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
            // If the user principal can't be extracted, redirect the user to the login page.
            if (!result.Succeeded)
            {
                return Challenge(
                    authenticationSchemes: JwtBearerDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties
                    {
                        RedirectUri = Request.PathBase + Request.Path + QueryString.Create(
                            Request.HasFormContentType ? Request.Form.ToList() : Request.Query.ToList())
                    });
            }
            var currentUserOpenId = result.Principal.GetClaim(Claims.Subject) ?? throw new Exception("用户标识不存在");

            // Retrieve the profile of the logged in user.(待确认，因为只使用openid和tenant，这里是否可以不查询?查询一次可以防止令牌失效等情况)
            //var resultdata = await _userLogin.GetLoginInfoAsync(result.Principal.GetClaim(Claims.Subject) ?? throw new Exception("用户标识不存在"),1) ?? throw new Exception("用户详细信息不存在");
            var resultdata = await _userLogin.GetLoginInfoAsync(currentUserOpenId, 1);
            if (!resultdata.IsSuccess) throw new Exception("用户详细信息不存在");
            resultdata.Tenant = result.Principal.GetClaim("tenant");//加上用户登录选择的租户信息

            // Retrieve the application details from the database.
            var application = await _applicationManager.FindByClientIdAsync(request.ClientId) ?? throw new InvalidOperationException("未查找到该客户端的应用详细信息");

            // Retrieve the permanent authorizations associated with the user and the calling client application.
            var authorizations = await _authorizationManager.FindAsync(
                  subject: currentUserOpenId,
                  //subject: resultdata.Content.Id.ToString(),
                  client: await _applicationManager.GetIdAsync(application) ?? throw new Exception("没有找到客户端的应用信息"), //这里区分下 是application的Id而不是ClientId
                  status: Statuses.Valid,
                  type: AuthorizationTypes.Permanent,
                  scopes: request.GetScopes()).ToListAsync();

            var consenttype = await _applicationManager.GetConsentTypeAsync(application);
            //获取授权同意确认页面
            switch (consenttype)
            {
                //判断授权同意的类型

                // If the consent is external (e.g when authorizations are granted by a sysadmin),
                // immediately return an error if no authorization can be found in the database.
                case ConsentTypes.External when !authorizations.Any():
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                                "登录用户没有访问该客户端应用的权限"
                        }));

                // If the consent is implicit or if an authorization was found,
                // return an authorization response without displaying the consent form.
                case ConsentTypes.Implicit:
                case ConsentTypes.External when authorizations.Any():
                case ConsentTypes.Explicit when authorizations.Any() && !request.HasPromptValue("Prompts.Consent"):

                    ClaimsPrincipal principal = CreateUserPrincpal(resultdata, TokenValidationParameters.DefaultAuthenticationType);
                    // Note: the granted scopes match the requested scope
                    // but you may want to allow the user to uncheck specific scopes.
                    // For that, simply restrict the list of scopes before calling SetScopes.
                    principal.SetScopes(request.GetScopes());

                    //查找scope允许访问的资源
                    var resources = await _scopeManager.ListResourcesAsync(principal.GetScopes()).ToListAsync();

                    principal.SetResources(resources);

                    // Automatically create a permanent authorization to avoid requiring explicit consent
                    // for future authorization or token requests containing the same scopes.                    
                    var authorization = authorizations.LastOrDefault();
                    if (authorization is null)
                    {
                        authorization = await _authorizationManager.CreateAsync(
                            principal: principal,
                            subject: currentUserOpenId,
                            //subject: resultdata.Content.Id.ToString(),
                            client: await _applicationManager.GetIdAsync(application),
                            type: AuthorizationTypes.Permanent,
                            scopes: principal.GetScopes());
                    }

                    principal.SetAuthorizationId(await _authorizationManager.GetIdAsync(authorization));

                    //foreach (var claim in principal.Claims)
                    //{
                    //    claim.SetDestinations(GetDestinations(claim));
                    //}

                    principal.SetDestinations(GetDestinations);

                    //登录   OpenIddict签发令牌,用户登录成功后颁发一个证书（加密的用户凭证），用来标识用户的身份。
                    return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                // At this point, no authorization was found in the database and an error must be returned
                // if the client application specified prompt=none in the authorization request.
                case ConsentTypes.Explicit when request.HasPromptValue("Prompts.None"):
                case ConsentTypes.Systematic when request.HasPromptValue("Prompts.None"):
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                                "Interactive user consent is required."
                        }));

                // In every other case, render the consent form.


                default:                    
                    //默认用户同意授权，不再出现询问用户是否同意授权的界面
                    return await DefaultAccept(request);

            }
            
            //// Retrieve the application details from the database.
            //var application = await _applicationManager.FindByClientIdAsync(request.ClientId) ??
            //    throw new InvalidOperationException("Details concerning the calling client application cannot be found.");
            //return View(new AuthorizeViewModel
            //{
            //    ApplicationName = await _applicationManager.GetDisplayNameAsync(application),
            //    Scope = request.Scope
            //});
        }
       
        /// <summary>
        /// 同意授权
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="Exception"></exception>
        [Authorize, FormValueRequired("submit.Accept")]
        [HttpPost("~/connect/authorize"), ValidateAntiForgeryToken]//authorize
        public async Task<IActionResult> Accept()
        {
            var request = HttpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            var resultdata = await _userLogin.GetLoginInfoAsync(User.GetClaim(Claims.Subject) ?? throw new Exception("The OpenID cannot be retrieved."), 1);
            if (!resultdata.IsSuccess) throw new Exception("The user details cannot be retrieved.");

            if(request.ClientId ==null )
            {
                throw new InvalidOperationException("The client id is null.");
            }
            //Retrieve the application details from the database.
            var application = await _applicationManager.FindByClientIdAsync(request.ClientId) ?? throw new InvalidOperationException("Details concerning the calling client application cannot be found.");

            //Retrieve the permanent authorizations associated with the user and the calling client application.
            var authorizations = await _authorizationManager.FindAsync(          
                  subject: resultdata.OpenId,
                  client: await _applicationManager.GetIdAsync(application) ?? throw new Exception("没有找到客户端的应用信息"), //这里区分下 是application的Id而不是ClientId
                  status: Statuses.Valid,
                  type: AuthorizationTypes.Permanent,
                  scopes: request.GetScopes()).ToListAsync();

            // Note: the same check is already made in the other action but is repeated
            // here to ensure a malicious user can't abuse this POST-only endpoint and
            // force it to return a valid response without the external authorization.
            if (!authorizations.Any() && await _applicationManager.HasConsentTypeAsync(application, ConsentTypes.External))
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The logged in user is not allowed to access this client application."
                    }));
            }
            ClaimsPrincipal principal = CreateUserPrincpal(resultdata);

            principal.SetScopes(request.GetScopes());
            principal.SetResources(await _scopeManager.ListResourcesAsync(principal.GetScopes()).ToListAsync());


            var authorization = authorizations.LastOrDefault();
            if (authorization is null)
            {

                authorization = await _authorizationManager.CreateAsync(
                    principal: principal,                    
                    subject: resultdata.OpenId,
                    client: await _applicationManager.GetIdAsync(application) ?? throw new Exception("未找到客户端应用信息"),
                    type: AuthorizationTypes.Permanent,
                    scopes: principal.GetScopes());
            }

            principal.SetAuthorizationId(await _authorizationManager.GetIdAsync(authorization));

            //foreach (var claim in principal.Claims)
            //{
            //    claim.SetDestinations(GetDestinations(claim, principal));
            //}
            principal.SetDestinations(GetDestinations);

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);


        }

        /// <summary>
        /// 拒绝授权
        /// </summary>
        /// <returns></returns>
        [Authorize, FormValueRequired("submit.Deny")]
        [HttpPost("~/connect/authorize"), ValidateAntiForgeryToken]//authorize
        // Notify OpenIddict that the authorization grant has been denied by the resource owner
        // to redirect the user agent to the client application using the appropriate response_mode.
        public IActionResult Deny() => Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        /// <summary>
        /// 默认直接同意授权，不再出现询问用户是否同意授权的界面
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="Exception"></exception>        
        public async Task<IActionResult> DefaultAccept(OpenIddictRequest request)
        {
            if (request == null) throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            var resultdata = await _userLogin.GetLoginInfoAsync(User.GetClaim(Claims.Subject) ?? throw new Exception("用户标识不存在"), 1);
            if (!resultdata.IsSuccess) throw new Exception("用户详细信息不存在");

            // 获取客户端详细信息 验证其他数据
            var application = await _applicationManager.FindByClientIdAsync(request.ClientId) ?? throw new InvalidOperationException("未查找到该客户端的应用详细信息");

            //查找当前情况客户端下请求用户的持久化授权数据信息
            var authorizations = await _authorizationManager.FindAsync(
              subject: resultdata.OpenId,
              //subject: resultdata.Content.Id.ToString(),
              client: await _applicationManager.GetIdAsync(application) ?? throw new Exception("没有找到客户端的应用信息"), //这里区分下 是application的Id而不是ClientId
              status: Statuses.Valid,
              type: AuthorizationTypes.Permanent,
              scopes: request.GetScopes()).ToListAsync();


            if (!authorizations.Any() && await _applicationManager.HasConsentTypeAsync(application, ConsentTypes.External))
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ConsentRequired,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            "The logged in user is not allowed to access this client application."
                    }));
            }
            ClaimsPrincipal principal = CreateUserPrincpal(resultdata);

            principal.SetScopes(request.GetScopes());
            principal.SetResources(await _scopeManager.ListResourcesAsync(principal.GetScopes()).ToListAsync());


            var authorization = authorizations.LastOrDefault();
            if (authorization is null)
            {

                authorization = await _authorizationManager.CreateAsync(
                    principal: principal,
                    //subject: resultdata.Content.Id.ToString(),
                    subject: resultdata.OpenId,
                    client: await _applicationManager.GetIdAsync(application) ?? throw new Exception("未找到客户端应用信息"),
                    type: AuthorizationTypes.Permanent,
                    scopes: principal.GetScopes());
            }

            principal.SetAuthorizationId(await _authorizationManager.GetIdAsync(authorization));

            //foreach (var claim in principal.Claims)
            //{
            //    claim.SetDestinations(GetDestinations(claim, principal));
            //}
            principal.SetDestinations(GetDestinations);

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        #endregion

        #region Logout support for interactive flows like code and implicit
        /// <summary>
        /// 登出系统--用户登出直接跳转，不点击页面确认按钮--备份
        /// </summary>
        /// <returns></returns>
        [HttpGet("~/connect/logout")]
        public IActionResult Logout()
        {
            return View("Logout_blank");
        }

        /// <summary>
        /// 登出系统清除本地cookies
        /// 以及把当前用户的未过期的access_token和refresh_token标记为Revoke
        /// </summary>
        /// <returns></returns>
        [ActionName(nameof(Logout)), HttpPost("~/connect/logout"), ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutPost()
        {
            try
            {
                var result = await HttpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
                var openid = result.Principal.GetClaim(Claims.Subject);

                await foreach (var dbtoken in _tokenManager.FindBySubjectAsync(openid))
                {
                    if (await _tokenManager.GetExpirationDateAsync(dbtoken) >= DateTimeOffset.UtcNow && await _tokenManager.HasStatusAsync(dbtoken, Statuses.Valid)
                        && await _tokenManager.HasTypeAsync(dbtoken, ImmutableArray.Create(TokenTypeHints.AccessToken, TokenTypeHints.RefreshToken))
                        && !IndependentLogoutAppList.Contains(await _tokenManager.GetApplicationIdAsync(dbtoken)))
                    {
                        await _tokenManager.TryRevokeAsync(dbtoken);
                    }
                }
                //退出登录，如清除Coookie等。
                await HttpContext.SignOutAsync();
                return SignOut(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,                    
                    properties: new AuthenticationProperties
                    {
                        RedirectUri = "/"
                    });
            }
            catch (Exception ex)
            {
                //待记录异常，只做本地退出
                await HttpContext.SignOutAsync();
                return SignOut(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties
                    {
                        RedirectUri = "/"
                    });
            }

        }
        #endregion

        #region Password, authorization code, device and refresh token flows
        /// <summary>
        /// 根据不同的客户端授权模式，返回OpenID Connect标准令牌
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPost("~/connect/token"), Produces("application/json")]
        public async Task<IActionResult> Exchange()
        {
            var request = HttpContext.GetOpenIddictServerRequest();
            if (request == null)
            {
                throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");
            }

            ClaimsPrincipal claimsPrincipal;

            if (request.IsClientCredentialsGrantType())//for machine-to-machine applications.
            {
                // Note: the client credentials are automatically validated by OpenIddict:
                // if client_id or client_secret are invalid, this action won't be invoked.
                if (string.IsNullOrWhiteSpace(request.ClientId))
                {
                    throw new InvalidOperationException("Invalid ClientId");
                }

                var application = await _applicationManager.FindByClientIdAsync(request.ClientId) ??
                    throw new InvalidOperationException("The application cannot be found.");

                // Create a new ClaimsIdentity containing the claims that
                // will be used to create an id_token, a token or a code.
                var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, Claims.Name, Claims.Role);

                var sessionId = _idGenerator.Generate();

                // Use the client_id as the subject identifier.
                identity.SetClaim(Claims.Subject, request.ClientId ?? throw new InvalidOperationException());
                identity.SetClaim(Claims.Name, await _applicationManager.GetDisplayNameAsync(application));
                identity.AddClaim(new Claim(Claims.Nonce, sessionId));

                
                //identity.SetDestinations(static claim => claim.Type switch
                //{
                //    // Allow the "name" claim to be stored in both the access and identity tokens
                //    // when the "profile" scope was granted (by calling principal.SetScopes(...)).
                //    Claims.Name when claim.Subject.HasScope(Scopes.Profile)
                //        => [Destinations.AccessToken, Destinations.IdentityToken],

                //    // Otherwise, only store the claim in the access tokens.
                //    _ => [Destinations.AccessToken]
                //});

                claimsPrincipal = new ClaimsPrincipal(identity);
                claimsPrincipal.SetClaim(Claims.Private.AccessTokenLifetime, GetTokenLifetime().TotalSeconds.ToString(CultureInfo.InvariantCulture));
                claimsPrincipal.SetScopes(request.GetScopes());
                foreach (var claim in claimsPrincipal.Claims)
                {
                    claim.SetDestinations(Destinations.AccessToken);
                }

                return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            if (request.IsPasswordGrantType())
            {
                UserLoginResultModel result = new UserLoginResultModel();
                if ((string?)request.GetParameter("loginType") == "message")
                {
                    //认证手机短信登录
                    var verifyCodeResult = SmsService.VerifyCode(request.Username, request.Password, "4");
                    if (verifyCodeResult)
                    {
                        result = await _userLogin.LoginAsync(new UserLoginModel()
                        {
                            PhoneNumber = request.Username,
                            VerificationCode = request.Password,
                            LoginMode = "PhoneNumberWithSms"
                        });
                    }
                    else
                    {
                        result.Errors.Add(new ErrorModel() { Code = "Failed", Description = "短信验证码认证失败" });
                    }
                }
                else
                {
                    //认证手机号密码登录
                    result = await _userLogin.LoginAsync(new UserLoginModel() { PhoneNumber = request.Username, Password = request.Password });
                }
                if (!result.Succeeded)
                {
                    return BadRequest(new
                    {
                        error = Errors.InvalidGrant,
                        description = string.Join(";", result.Errors.Select(e => e.Description).ToList())
                    });
                    //throw new OpenIddictExceptions.ValidationException(string.Join(";", result.Errors.Select(e => e.Description).ToList()));
                    //return Forbid(
                    //    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    //    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    //    {
                    //        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                    //        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = string.Join(";", result.Errors.Select(e => e.Description).ToList())
                    //    }));
                }

                //var resultdata = result.Content as UserProfile ?? throw new Exception("登录用户信息异常");
                var resultdata = new UserInfoResult() { OpenId = result.OpenId, Tenants = result.Tenants, Tenant = request.GetParameter("tenant").ToString() };

                var principal = CreateUserPrincpal(resultdata);


                //这是密码模式能访问那些资源 //这里写死了全部返回，可考虑改为根据请求端principal.SetScopes(request.GetScopes());
                principal.SetScopes(new[]
               {
                        Scopes.OpenId,
                        Scopes.Email,
                        Scopes.Profile,
                        Scopes.Roles,
                        "user_api",
                        "openiddict_api",
                        Scopes.OfflineAccess

                    });

                principal.SetResources(await _scopeManager.ListResourcesAsync(principal.GetScopes()).ToListAsync());

                //foreach (var claim in principal.Claims)
                //{
                //    claim.SetDestinations(GetDestinations(claim, principal));
                //}
                principal.SetDestinations(GetDestinations);

                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            if (request.IsAuthorizationCodeGrantType() || request.IsDeviceCodeGrantType() || request.IsRefreshTokenGrantType())
            {
                // Retrieve the claims principal stored in the authorization code/device code/refresh token.
                //验证在 SignInAsync 中颁发的证书，并返回一个 AuthenticateResult 对象，表示用户的身份
                var principal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;
                //租户参数存在且与当前的租户不同，则用租户参数替换当前租户
                var tenantNew = (string?)request.GetParameter("tenant");
                if (tenantNew != null)
                {
                    var tenantOld = principal?.GetClaim("tenant").ToString();
                    if (tenantNew != tenantOld)
                    {
                        principal?.SetClaim("tenant", tenantNew);
                    }
                }


                // principal.Identity.IsAuthenticated
                // Retrieve the user profile corresponding to the authorization code/refresh token.
                var userIdentity = principal?.GetClaim(Claims.Subject);
                if (userIdentity == null)
                {
                    return BadRequest(new
                    {
                        error = Errors.InvalidGrant,
                        description = "用户唯一标识不存在"
                    });
                }
                var user = await _userLogin.GetLoginInfoAsync(userIdentity, 1);
                if (!user.IsSuccess)
                {
                    return BadRequest(new
                    {
                        error = Errors.InvalidGrant,
                        description = "无法获取用户信息"
                    });
                    //return Forbid(
                    //    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    //    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    //    {
                    //        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                    //        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "令牌已失效"
                    //    }));
                }
                principal.SetScopes(new[]
                  {
                        Scopes.OpenId,
                        Scopes.Profile,
                        Scopes.Roles,
                        Scopes.OfflineAccess,
                        Scopes.Phone

                    });
                principal.SetDestinations(GetDestinations);

                // Returning a SignInResult will ask OpenIddict to issue the appropriate access/identity tokens.
                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            throw new InvalidOperationException("The specified grant type is not supported.");
            //return BadRequest(new
            //{
            //    error = Errors.InvalidGrant,
            //    description = "The specified grant type is not supported."
            //});
        }
        #endregion

        #region Device flow
        /// <summary>
        /// 设备登录验证
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        // Note: to support the device flow, you must provide your own verification endpoint action:
        [Authorize, HttpGet("~/connect/verify"), IgnoreAntiforgeryToken]
        public async Task<IActionResult> Verify()
        {

            var request = HttpContext.GetOpenIddictServerRequest() ??
                throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            // If the user code was not specified in the query string (e.g as part of the verification_uri_complete),
            // render a form to ask the user to enter the user code manually (non-digit chars are automatically ignored).
            if (string.IsNullOrEmpty(request.UserCode))
            {
                return View(new VerifyViewModel());
            }

            // Retrieve the claims principal associated with the user code.
            var result1 = await HttpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
            var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            if (result.Succeeded && !string.IsNullOrEmpty(result.Principal.GetClaim(Claims.ClientId)))
            {
                // Retrieve the application details from the database using the client_id stored in the principal.
                //这里的ClientId内容在登录获取cookies信息的时候进行录入
                var application = await _applicationManager.FindByClientIdAsync(result.Principal.GetClaim(Claims.ClientId)) ??
                    throw new InvalidOperationException("未查找到该客户端应用详细信息");

                // Render a form asking the user to confirm the authorization demand.
                return View(new VerifyViewModel
                {
                    ApplicationName = await _applicationManager.GetLocalizedDisplayNameAsync(application),
                    Scope = string.Join(" ", result.Principal.GetScopes()),
                    UserCode = request.UserCode
                });
            }

            // Redisplay the form when the user code is not valid.
            return View(new VerifyViewModel
            {
                Error = Errors.InvalidToken,
                ErrorDescription = "The specified user code is not valid. Please make sure you typed it correctly."
            });
        }

        

        [Authorize, FormValueRequired("submit.Accept")]
        [HttpPost("~/connect/verify"), ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyAccept()
        {
            // Retrieve the profile of the logged in user.
            var user = await _userLogin.GetLoginInfoAsync(User.GetClaim(Claims.Subject) ?? throw new Exception("The OpenID cannot be retrieved."), 1) ??
                throw new InvalidOperationException("The user details cannot be retrieved.");

            // Retrieve the claims principal associated with the user code.
            var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            if (result.Succeeded)
            {
                // Create the claims-based identity that will be used by OpenIddict to generate tokens.
                //var identity = new ClaimsIdentity(
                //    authenticationType: TokenValidationParameters.DefaultAuthenticationType,
                //    nameType: Claims.Name,
                //    roleType: Claims.Role);

                //// Add the claims that will be persisted in the tokens.
                //identity.SetClaim(Claims.Subject, await _userManager.GetUserIdAsync(user))
                //        .SetClaim(Claims.Email, await _userManager.GetEmailAsync(user))
                //        .SetClaim(Claims.Name, await _userManager.GetUserNameAsync(user))
                //        .SetClaims(Claims.Role, (await _userManager.GetRolesAsync(user)).ToImmutableArray());

                //await AddUserClaimsAsync(identity, user);
                var identity = CreateUserIdentity(user);

                // Note: in this sample, the granted scopes match the requested scope
                // but you may want to allow the user to uncheck specific scopes.
                // For that, simply restrict the list of scopes before calling SetScopes.
                identity.SetScopes(result.Principal.GetScopes());
                identity.SetResources(await _scopeManager.ListResourcesAsync(identity.GetScopes()).ToListAsync());
                identity.SetDestinations(GetDestinations);

                var properties = new AuthenticationProperties
                {
                    // This property points to the address OpenIddict will automatically
                    // redirect the user to after validating the authorization demand.
                    RedirectUri = "/pauth"
                };

                return SignIn(new ClaimsPrincipal(identity), properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            // Redisplay the form when the user code is not valid.
            return View(new VerifyViewModel
            {
                Error = Errors.InvalidToken,
                ErrorDescription = "The specified user code is not valid. Please make sure you typed it correctly."
            });
        }

        [Authorize, FormValueRequired("submit.Deny")]
        [HttpPost("~/connect/verify"), ValidateAntiForgeryToken]
        // Notify OpenIddict that the authorization grant has been denied by the resource owner.
        public IActionResult VerifyDeny() => Forbid(
            authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            properties: new AuthenticationProperties()
            {
                // This property points to the address OpenIddict will automatically
                // redirect the user to after rejecting the authorization demand.
                RedirectUri = "/pauth"
            });
        #endregion

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
        [HttpGet("~/connect/userinfo"), HttpPost("~/connect/userinfo")]
        [IgnoreAntiforgeryToken, Produces("application/json")]
        public async Task<IActionResult> Userinfo()
        {
            var claimsPrincipal = (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;

            //获取当前token，获取 AuthenticationProperties 中保存的额外信息,使用当前token向用户管理服务请求用户基本信息
            var token = await HttpContext.GetTokenAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, "access_token");
            var user = await _userLogin.GetUserInfoAsync(token);
            if (!user.IsSuccess)
            {
                return Challenge(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidToken,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                        "The specified access token is bound to an account that no longer exists."                        
                    }));
            }

            var claims = new Dictionary<string, object>(StringComparer.Ordinal)
            {
                // Note: the "sub" claim is a mandatory claim and must be included in the JSON response.
                [Claims.Subject] = claimsPrincipal.FindFirstValue(Claims.Subject),//user.OpenId, //await _userManager.GetUserIdAsync(user)
                ["tenant"] = claimsPrincipal.FindFirstValue("tenant"),
                //[Claims.PhoneNumber] = user.PhoneNumber,
                [Claims.Nickname] = user.NickName ?? ""
            };

            if (User.HasScope(Scopes.Profile))
            {
                claims[Claims.PhoneNumber] = user.PhoneNumber ?? "";//claimsPrincipal.FindFirstValue(Claims.PhoneNumber);//await _userManager.GetPhoneNumberAsync(user);
                claims[Claims.PhoneNumberVerified] = true;//await _userManager.IsPhoneNumberConfirmedAsync(user);
            }
                        

            // Note: the complete list of standard claims supported by the OpenID Connect specification
            // can be found here: http://openid.net/specs/openid-connect-core-1_0.html#StandardClaims

            return Ok(claims);
        }

        public IActionResult Index()
        {
            return View();
        }

        private TimeSpan GetTokenLifetime()
        {
            var minutes = _options.Value.AccessTokenLifetime;
            var delay = _randomGenerator.Next(0, minutes);
            return TimeSpan.FromMinutes(minutes + delay);
        }

        private IEnumerable<string> GetDestinations(Claim claim)
        {
            // Note: by default, claims are NOT automatically included in the access and identity tokens.
            // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
            // whether they should be included in access tokens, in identity tokens or in both.

            switch (claim.Type)
            {
                case Claims.Subject:
                    yield return Destinations.AccessToken;

                    yield return Destinations.IdentityToken;

                    yield break;
                case "tenant":
                    yield return Destinations.AccessToken;

                    yield return Destinations.IdentityToken;

                    yield break;
                case "open_id":
                    yield return Destinations.AccessToken;

                    yield return Destinations.IdentityToken;

                    yield break;
                case Claims.PhoneNumber:
                    yield return Destinations.AccessToken;

                    if (claim.Subject.HasScope(Scopes.Profile))
                        yield return Destinations.IdentityToken;

                    yield break;
                case Claims.Username:
                    yield return Destinations.AccessToken;

                    if (claim.Subject.HasScope(Scopes.Profile))
                        yield return Destinations.IdentityToken;

                    yield break;
                case Claims.Name:
                    yield return Destinations.AccessToken;

                    if (claim.Subject.HasScope(Scopes.Profile))
                        yield return Destinations.IdentityToken;

                    yield break;

                //case Claims.Email:
                //    yield return Destinations.AccessToken;

                //    if (claim.Subject.HasScope(Scopes.Email))
                //        yield return Destinations.IdentityToken;

                //    yield break;

                //case Claims.Role:
                //    yield return Destinations.AccessToken;

                //    if (claim.Subject.HasScope(Scopes.Roles))
                //        yield return Destinations.IdentityToken;

                //    yield break;

                // Never include the security stamp in the access and identity tokens, as it's a secret value.
                case "AspNet.Identity.SecurityStamp": yield break;

                default:
                    yield return Destinations.AccessToken;
                    yield break;
            }
        }


        private ClaimsPrincipal CreateUserPrincpal(UserInfoResult resultdata, string claimsIdentityName = "AuthenticationTypes.Federation")        
        {
            //登录成功流程
            return new ClaimsPrincipal(CreateUserIdentity(resultdata, claimsIdentityName));
        }
        
        private ClaimsIdentity CreateUserIdentity(UserInfoResult resultdata, string authenticationType = "AuthenticationTypes.Federation")
        {
            var identity = new ClaimsIdentity(authenticationType);
            identity.AddClaim(new Claim(Claims.Subject, resultdata.OpenId));
            identity.AddClaim(new Claim("open_id", resultdata.OpenId));//提供给用户管理系统获取用户信息，可考虑统一为claims.subject
            identity.AddClaim(new Claim("tenant", string.IsNullOrWhiteSpace(resultdata.Tenant) ? string.Empty : resultdata.Tenant));
            //identity.AddClaim(new Claim(Claims.Nickname, resultdata.NickName));
            //identity.AddClaim(new Claim(Claims.ClientId, clientId));
            //identity.AddClaim(new Claim("tenantid", resultdata.CompanyId + ""));
            //identity.AddClaim(new Claim("organizes", resultdata.DepartmentCode + ""));
            //identity.AddClaim(new Claim("usergroups", resultdata.UserGroupCode));
            //identity.AddClaim(new Claim("usertype", resultdata.Role + ""));
            //identity.AddClaim(new Claim("userroles", resultdata.Role + ""));
            //identity.AddClaim(new Claim("userposts", resultdata.Role + ""));
            return identity;
        }
    }
}

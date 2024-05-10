using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;


namespace TongBuilder.Application.Server.Auth
{    
    /// <summary>
    /// 客户端身份验证状态提供程序仅在 Blazor 中使用，不与 ASP.NET Core 身份验证系统集成。 
    /// 在预呈现期间，Blazor 尊重页面上定义的元数据，并使用 ASP.NET Core 身份验证系统，
    /// 以确定用户是否已通过身份验证。 当用户从一个页面导航到另一个页面时，
    /// 将使用客户端身份验证提供程序。 当用户刷新页面（重新加载整个页面）时，
    /// 客户端身份验证状态提供程序不参与服务器上的身份验证决策。 
    /// 由于服务器没有持久化用户的状态，因此客户端维护的任何身份验证状态都将丢失。
    /// 最佳方法是在 ASP.NET Core 身份验证系统内执行身份验证。
    /// 客户端身份验证状态提供程序只负责反映用户的身份验证状态。
    /// </summary>
    public static class JwtServiceCollectionExtensions
    {
        const string MS_OIDC_SCHEME = "MicrosoftOidc";

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // 读取配置
            services.AddOptions<JwtSetting>();
            services.Configure<JwtSetting>(configuration);
            var symmetricKeyAsBase64 = configuration["Secret"]?? "25DFF2D613584EFBA8F8025DBE0997B5";
            var issuer = configuration["Issuer"];
            var audience = configuration["Audience"];

            // 获取密钥
            var keyByteArray = Encoding.UTF8.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            // 令牌验证参数
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true, // 是否验证SecurityKey
                IssuerSigningKey = signingKey, // 拿到SecurityKey签名秘钥
                ValidateIssuer = true, // 是否验证Issuer
                ValidIssuer = issuer, // 发行人Issuer
                ValidateAudience = true, // 是否验证Audience
                ValidAudience = audience, // 订阅人Audience
                ValidateLifetime = true, // 是否验证失效时间
                ClockSkew = TimeSpan.FromSeconds(30), // 过期时间容错值，解决服务器端时间不同步问题（秒）
                RequireExpirationTime = true,
            };
            //IssuerSigningKey(签名秘钥), ValidIssuer(Token颁发机构), ValidAudience(颁发给谁) 三个参数是必须的，后两者用于与TokenClaims中的Issuer和Audience进行对比，不一致则验证失败
            // events,服务器可以用来针对客户端的请求发送质询(challenge)，客户端根据质询提供身份验证凭证。
            var jwtBearerEvents = new JwtBearerEvents()
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Token;                                       
                    
                    return Task.CompletedTask;
                },
                OnChallenge = async context =>
                {
                    // refresh token

                    // 
                    context.Response.Redirect("user/login");

                    //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    //context.Response.ContentType = "application/json";
                    //await context.Response.WriteAsync("401");

                    // 标识处理了响应
                    context.HandleResponse();
                },
                OnForbidden = async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("403");
                },
                OnTokenValidated = async context =>
                {
                    if (context.Request.Path.Value.ToString() == "/api/logout")
                    {
                        var token = ((context as Microsoft.AspNetCore.Authentication.JwtBearer.TokenValidatedContext).SecurityToken as JwtSecurityToken).RawData;
                        
                    }
                    
                }
            };

            // 开启Bearer认证
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;//对应会执行JwtBearerHandler的HandleChallengeAsync方法
                //options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
                //options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                //options.Authority = "http://localhost:5277/";//OIDC服务的地址，可以自动发现Issuer, IssuerSigningKey等配置
                //options.Audience = "TongBuilder";
                options.TokenValidationParameters = tokenValidationParameters;
                options.Events = jwtBearerEvents;
            });
            //从OIDC服务器获取Token，然后保存到sesstionStorage，在发送请求时附加到请求头
            return services;
        }
        /// <summary>
        /// Webassembly使用
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddOidcAuthentication_(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOidcAuthentication(options =>
            {
                configuration.Bind("Identity", options.ProviderOptions);
            });
            return services;
        }
        /// <summary>
        /// ASP.NET Core 身份验证系统
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddOpenIdConnectAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(MS_OIDC_SCHEME,oidcOptions =>
                {
                    //设置与身份验证成功后负责保留用户标识的中间件对应的身份验证方案。 OIDC 处理程序需要使用能够跨请求保留用户凭据的登录方案。
                    oidcOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    oidcOptions.ClientId = "your_client_id";
                    oidcOptions.ClientSecret = "your_client_secret";
                    oidcOptions.Authority = "https://your_authority_url/";
                    oidcOptions.ResponseType = OpenIdConnectResponseType.Code;//OIDC 处理程序使用从授权终结点返回的代码自动请求适当的令牌。
                    oidcOptions.SaveTokens = true;//定义在授权成功后，是否应在 AuthenticationProperties 中存储访问令牌和刷新令牌。 
                    oidcOptions.GetClaimsFromUserInfoEndpoint = true;
                    oidcOptions.Scope.Add("openid");
                    oidcOptions.Scope.Add("profile");
                    oidcOptions.Scope.Add("email");
                    oidcOptions.Scope.Add(OpenIdConnectScope.OfflineAccess);
                    oidcOptions.MapInboundClaims = false;
                    oidcOptions.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
                    oidcOptions.TokenValidationParameters.RoleClaimType = "role";
                    oidcOptions.CallbackPath = new PathString("/signin-oidc");
                    oidcOptions.SignedOutCallbackPath = new PathString("/signout-callback-oidc");
                    oidcOptions.RemoteSignOutPath = new PathString("/signout-oidc");
                });
            // ConfigureCookieOidcRefresh attaches a cookie OnValidatePrincipal callback to get
            // a new access token when the current one expires, and reissue a cookie with the
            // new access token saved inside. If the refresh fails, the user will be signed
            // out. OIDC connect options are set for saving tokens and the offline access
            // scope.
            services.ConfigureCookieOidcRefresh(CookieAuthenticationDefaults.AuthenticationScheme, MS_OIDC_SCHEME);
            return services;
        }
    }
}

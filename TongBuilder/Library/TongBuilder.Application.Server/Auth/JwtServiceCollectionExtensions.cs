using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // 读取配置
            services.AddOptions<JwtSetting>();
            services.Configure<JwtSetting>(configuration);
            var symmetricKeyAsBase64 = configuration["Secret"];
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
                .AddCookie()
                .AddOpenIdConnect(options =>
                {
                    options.ClientId = "your_client_id";
                    options.ClientSecret = "your_client_secret";
                    options.Authority = "https://your_authority_url/";
                    options.ResponseType = "code";
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("email");
                });
            return services;
        }
    }
}

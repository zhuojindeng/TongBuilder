using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Quartz;
using static OpenIddict.Abstractions.OpenIddictConstants;
using System.Security.Cryptography;
using System.Text;
using TongBuilder.SSOServer.Constants;
using TongBuilder.SSOServer.Options;

namespace TongBuilder.SSOServer.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenIdConnect(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetSection("Auth");
            services.AddOptions<AuthOptions>()
                .ValidateDataAnnotations();
            services.Configure<AuthOptions>(config);

            var issuer = config.GetValue<string>("Issuer", "");

            // OpenIddict offers native integration with Quartz.NET to perform scheduled tasks
            // (like pruning orphaned authorizations/tokens from the database) at regular intervals.
            services.AddQuartz(options =>
            {                
                options.UseSimpleTypeLoader();
                options.UseInMemoryStore();
            });
            // Register the Quartz.NET service and configure it to block shutdown until jobs are complete.
            services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
            services.AddOpenIddict()
                    // Register the OpenIddict core components.
                    .AddCore(options =>
                    {
                        options.UseMongoDb().UseDatabase(new MongoClient(configuration.GetConnectionString("DefaultConnection")).GetDatabase("openiddict"));

                        options.UseQuartz();
                    })
                    // Register the OpenIddict server components.
                    .AddServer(options =>
                    {
                        // Enable the authorization and token endpoints.
                        options.SetAuthorizationEndpointUris("connect/authorize")
                               .SetTokenEndpointUris("connect/token")
                               .SetDeviceEndpointUris("/connect/device")
                               .SetIntrospectionEndpointUris("/connect/introspect")
                               //.SetRevocationEndpointUris("/connect/revocat")
                               .SetUserinfoEndpointUris("/connect/userinfo")
                               .SetVerificationEndpointUris("/connect/verify")
                               .SetLogoutEndpointUris("/connect/logout");

                        // Enable the authorization code flow.
                        options.AllowAuthorizationCodeFlow()
                                 //.RequireProofKeyForCodeExchange()//this makes sure all clients are required to use PKCE (Proof Key for Code Exchange).
                                 .AllowClientCredentialsFlow()
                                 .AllowDeviceCodeFlow()
                                 .AllowHybridFlow()
                                 .AllowImplicitFlow()
                                 .AllowPasswordFlow()
                                 .AllowRefreshTokenFlow()
                                 // Encryption and signing of tokens
                                 //.AddEphemeralEncryptionKey()
                                 //.AddEphemeralSigningKey()

                                 // Register scopes (permissions)
                                 .RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles, "api")

                                 .Configure(options =>
                                 {
                                     options.CodeChallengeMethods.Add(CodeChallengeMethods.Plain);
                                 })
                                  // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                                  .UseAspNetCore().EnableStatusCodePagesIntegration()
                                                   .EnableAuthorizationEndpointPassthrough()
                                                   .EnableLogoutEndpointPassthrough()
                                                   .EnableTokenEndpointPassthrough()
                                                   .EnableUserinfoEndpointPassthrough()
                                                   .EnableVerificationEndpointPassthrough()
                                                   .DisableTransportSecurityRequirement(); // 禁用HTTPS 在开发测试环境;

                        // Register the signing and encryption credentials.发布正式版本需要替换
                        //options.AddDevelopmentEncryptionCertificate()
                        //       .AddDevelopmentSigningCertificate();

                        //token签名秘钥
                        options.AddSigningCredentials(
                            new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Keys.IssuerSigningKey)), SecurityAlgorithms.HmacSha256));

                        //加密秘钥
                        options.AddEncryptionCredentials(
                        new EncryptingCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Keys.IssuerEncryptingKey)), SecurityAlgorithms.Aes256KW, SecurityAlgorithms.Aes256CbcHmacSha512));

                        //根据openiddict框架要求，必须配置一个非对称秘钥，但优先使用对称秘钥作为签名秘钥
                        RSA rSA = RSA.Create();
                        var privateKeyBytes = Convert.FromBase64String(Keys.RsaPrivateKey);
                        rSA.ImportPkcs8PrivateKey(privateKeyBytes, out int _);
                        RsaSecurityKey securitykey = new RsaSecurityKey(rSA);

                        options.AddSigningCredentials(
                            new SigningCredentials(new RsaSecurityKey(rSA), SecurityAlgorithms.RsaSha256));
                        //options.DisableTokenStorage();
                        //暂时不启用accesstoken加密，只采用最简单的对称加密，后期可根据实际情况进行修改
                        options.DisableAccessTokenEncryption();
                        if (!string.IsNullOrWhiteSpace(issuer))
                        {
                            options.SetIssuer(new Uri(issuer));
                        }
                    })
                    // Register the OpenIddict validation components.
                    .AddValidation(options =>
                    {
                        // Import the configuration from the local OpenIddict server instance.
                        options.UseLocalServer();
                        // For applications that need immediate access token or authorization
                        // revocation, the database entry of the received tokens and their
                        // associated authorizations can be validated for each API call.
                        // Enabling these options may have a negative impact on performance.
                        //options.EnableAuthorizationEntryValidation();
                        
                        // Register the ASP.NET Core host.
                        options.UseAspNetCore();

                        //for client
                        // Instead of validating the token locally by reading it directly,
                        // introspection can be used to ask a remote authorization server
                        // to validate the token (and its attached database entry).
                        //
                        //options.UseIntrospection()                               
                        //       .SetIssuer(new Uri(issuer))
                        //       .SetClientId("resource_server")
                        //       .SetClientSecret("80B552BB-4CD8-48DA-946E-0815E0147DD2");
                        //
                        // When introspection is used, System.Net.Http integration must be enabled.
                        //
                        //options.UseSystemNetHttp();

                    });
           return services;
        }
    }
}

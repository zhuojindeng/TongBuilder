using Microsoft.AspNetCore.Authentication.JwtBearer;
using TongBuilder.SSOServer.DependencyInjection;
using TongBuilder.AuthProxy.DependencyInjection;

Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

var configuration = configBuilder.Build();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        //ValidAudience = authSettings.Audience,
        //ValidIssuer = authSettings.Issuer,
        //IssuerSigningKey = issuerSigningKey,
#if DEBUG
        ClockSkew = TimeSpan.FromSeconds(2), //Default is 300 seconds. This is for testing the correctness of the auth protocol implementation between C/S.
#endif
    }; 
});

builder.Services.AddAuthProxy(configuration);

builder.Services.AddOpenIdConnect(configuration);



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

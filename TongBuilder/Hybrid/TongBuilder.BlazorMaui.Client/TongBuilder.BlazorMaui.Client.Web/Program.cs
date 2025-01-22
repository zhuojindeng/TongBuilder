using TongBuilder.BlazorMaui.Client.Shared.Services;
using TongBuilder.BlazorMaui.Client.Web.Components;
using TongBuilder.BlazorMaui.Client.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Add device-specific services used by the TongBuilder.BlazorMaui.Client.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        typeof(TongBuilder.BlazorMaui.Client.Shared._Imports).Assembly,
        typeof(TongBuilder.BlazorMaui.Client.Web.Client._Imports).Assembly);

app.Run();

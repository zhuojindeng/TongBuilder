using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TongBuilder.BlazorMaui.Client.Shared.Services;
using TongBuilder.BlazorMaui.Client.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add device-specific services used by the TongBuilder.BlazorMaui.Client.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

await builder.Build().RunAsync();

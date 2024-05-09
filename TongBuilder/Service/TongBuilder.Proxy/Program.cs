var builder = WebApplication.CreateBuilder(args);
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
var app = builder.Build();

app.MapReverseProxy(proxyPipeline =>
{
    proxyPipeline.Use((context, next) =>
    {
        // Custom logic before forwarding the request
        // ...

        return next();
    });

    // Additional middleware can be added here
});

app.MapGet("/", () => "Hello World!");

app.Run();

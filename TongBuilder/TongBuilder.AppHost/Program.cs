var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.TongBuilder_BlazorWeb>("tongbuilder-blazorweb");

builder.Build().Run();

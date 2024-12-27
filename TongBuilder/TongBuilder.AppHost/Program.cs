var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.TongBuilder_BlazorWeb>("tongbuilder-blazorweb");

builder.AddProject<Projects.TongBuilder_Test_WebApi>("tongbuilder-test-webapi");

builder.AddProject<Projects.TongBuilder_AI>("tongbuilder-ai");

builder.Build().Run();

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.FriterieShop_API>("apiservice");

builder.AddProject<Projects.FriterieShop_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();

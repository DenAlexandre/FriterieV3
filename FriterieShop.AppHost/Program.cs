var builder = DistributedApplication.CreateBuilder(args);



// Add a Docker Compose environment
var compose = builder.AddDockerComposeEnvironment("compose");


//var cache = builder.AddRedis("cache")
//                   .PublishAsDockerComposeService((resource, service) =>
//                   {
//                       service.Name = "cache";
//                   });

// PostgreSQL
//var postgres = builder.AddPostgres("postgres")
//    .WithImage("postgres:latest")
//    .WithHostPort(5432)
//    .WithEnvironment("POSTGRES_DB", "db_shop")
//    .WithEnvironment("POSTGRES_USER", "postgres")
//    .WithEnvironment("POSTGRES_PASSWORD", "postgres")
//    .WithDataVolume(); // persistance des données

var postgres = builder.AddConnectionString("DefaultConnection");


var apiService = builder.AddProject<Projects.FriterieShop_API>("apiservice")
                        .WithReference(postgres)
                        .PublishAsDockerComposeService((resource, service) =>
                        {
                            service.Name = "api";
                        });

var webApp = builder.AddProject<Projects.FriterieShop_Web>("webfrontend")
                    //.WithReference(cache)
                    .WithReference(apiService)
                    .WaitFor(apiService)
                    .PublishAsDockerComposeService((resource, service) =>
                    {
                        service.Name = "web";
                    });

builder.Build().Run();
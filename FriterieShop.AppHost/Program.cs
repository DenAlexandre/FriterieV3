var builder = DistributedApplication.CreateBuilder(args);



// Utilise le container PostgreSQL existant (docker-compose shop_postgres)
// via la connection string "DefaultConnection" de appsettings.json
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

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithEnvironment("POSTGRES_PASSWORD", "postgres")
    .WithEnvironment("POSTGRES_USER", "postgres");

builder.AddProject<Projects.WebApi>("AuctionWebApi")
    .WithReference(postgres)
    .WithEnvironment("ConnectionStrings__DefaultConnection", "$(postgres:connectionString)");

await builder.Build().RunAsync();
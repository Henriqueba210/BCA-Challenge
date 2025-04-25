var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");

builder.AddProject<Projects.WebApi>("AuctionWebApi")
    .WithReference(postgres)
    .WithEnvironment("ConnectionStrings__DefaultConnection", "$(postgres:connectionString)");

await builder.Build().RunAsync();
var builder = DistributedApplication.CreateBuilder(args);

var postgresdb = builder.AddPostgres("postgresdb");

builder.AddProject<Projects.WebApi>("AuctionWebApi")
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

await builder.Build().RunAsync();
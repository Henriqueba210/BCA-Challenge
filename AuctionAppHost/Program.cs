var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.WebApi>("AuctionWebApi");

builder.Build().Run();
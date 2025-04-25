using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

using Auction.Api;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace WebApi.Integration.Test;

public class AuctionApiFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly IHost _app;
    public IResourceBuilder<PostgresServerResource> Postgres { get; }
    private string? _postgresConnectionString;
    
    public AuctionApiFixture()
    {
        var options = new DistributedApplicationOptions { AssemblyName = typeof(AuctionApiFixture).Assembly.FullName, DisableDashboard = true };
        var appBuilder = DistributedApplication.CreateBuilder(options);
        Postgres = appBuilder.AddPostgres("postgres");
        _app = appBuilder.Build();
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                { "ConnectionStrings:postgresdb", _postgresConnectionString ?? "" },
            }!);
        });
        return base.CreateHost(builder);
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await _app.StopAsync();
        if (_app is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync().ConfigureAwait(false);
        }
        else
        {
            _app.Dispose();
        }
    }

    public async Task InitializeAsync()
    {
        await _app.StartAsync();
        _postgresConnectionString = await Postgres.Resource.GetConnectionStringAsync() ?? "";

        // Wait for Postgres to be ready
        var optionsBuilder = new DbContextOptionsBuilder<Auction.Infrastructure.AuctionDbContext>()
            .UseNpgsql(_postgresConnectionString);

        const int maxRetries = 10;
        const int delayMs = 1000;
        int retries = 0;
        while (true)
        {
            try
            {
                await using var context = new Auction.Infrastructure.AuctionDbContext(optionsBuilder.Options);
                await context.Database.OpenConnectionAsync();
                await context.Database.CloseConnectionAsync();
                break; // Connection successful
            }
            catch
            {
                retries++;
                if (retries >= maxRetries)
                    throw;
                await Task.Delay(delayMs);
            }
        }

        // Ensure Database is created and migrations are applied
        await using var migrationContext = new Auction.Infrastructure.AuctionDbContext(optionsBuilder.Options);
        await migrationContext.Database.MigrateAsync();
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Auction.Infrastructure.Repositories;

namespace Auction.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, bool useInMemory = false)
    {
        if (useInMemory)
        {
            services.AddDbContext<AuctionDbContext>(options =>
                options.UseInMemoryDatabase("AuctionDb"));
        }
        else
        {
            var connectionString = configuration.GetConnectionString("Postgres") ??
                                   throw new InvalidOperationException("Postgres connection string not found.");
            services.AddDbContext<AuctionDbContext>(options =>
                options.UseNpgsql(connectionString));
        }

        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<IAuctionRepository, AuctionRepository>();
        return services;
    }
}

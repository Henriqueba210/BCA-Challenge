using Auction.Application.Features.Auction.Abstractions;
using Auction.Application.Features.Vehicle.Abstractions;
using Auction.Infrastructure.Models; // Add this using if not present
using Auction.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auction.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, bool useInMemory = false)
    {
        // Register AuctionDbContext with PostgreSQL or InMemory database
        if (useInMemory)
        {
            services.AddDbContext<AuctionDbContext>(options =>
                options.UseInMemoryDatabase("AuctionDbTest"));
        }
        else
        {
            services.AddDbContext<AuctionDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("postgresdb")
                    ?? throw new InvalidOperationException("Connection string 'postgresdb' not found.")));
        }

        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<IAuctionRepository, AuctionRepository>();
        return services;
    }
}

using Auction.Application.Features.Auction.Abstractions;
using Auction.Application.Features.Vehicle.Abstractions;
using Auction.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auction.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, bool useInMemory = false)
    {
        if (useInMemory)
        {
            services.AddDbContext<AuctionDbContext>(options =>
                options.UseInMemoryDatabase("AuctionDbTest"));
        }
        else
        {
            services.AddDbContext<AuctionDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("postgresdb")
                    ?? configuration.GetConnectionString("DefaultConnection")));
        }

        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<IAuctionRepository, AuctionRepository>();
        return services;
    }
}

using Auction.Infrastructure.Models;

using Infrastructure.Entities;

using Microsoft.EntityFrameworkCore;

namespace Auction.Infrastructure;

public class AuctionDbContext(DbContextOptions<AuctionDbContext> options) : DbContext(options)
{
    public DbSet<VehicleEntity> Vehicles => Set<VehicleEntity>();
    public DbSet<SedanEntity> Sedans => Set<SedanEntity>();
    public DbSet<HatchbackEntity> Hatchbacks => Set<HatchbackEntity>();
    public DbSet<SUVEntity> SUVs => Set<SUVEntity>();
    public DbSet<TruckEntity> Trucks => Set<TruckEntity>();
    public DbSet<AuctionEntity> Auctions => Set<AuctionEntity>();
    public DbSet<BidEntity> Bids => Set<BidEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuctionDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

using Infrastructure.Entities;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class AuctionDbContext : DbContext
{
    public AuctionDbContext(DbContextOptions<AuctionDbContext> options) : base(options) { }

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

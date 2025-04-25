using Auction.Domain.Entities;
using Auction.Infrastructure.Models;

using Microsoft.EntityFrameworkCore;

namespace Auction.Infrastructure;

public class AuctionDbContext : DbContext
{
    public AuctionDbContext(DbContextOptions<AuctionDbContext> options)
        : base(options)
    {
    }

    public DbSet<SedanEntity> Sedans { get; set; }
    public DbSet<HatchbackEntity> Hatchbacks { get; set; }
    public DbSet<SuvEntity> SUVs { get; set; }
    public DbSet<TruckEntity> Trucks { get; set; }
    public DbSet<Models.AuctionEntity> Auctions => Set<Models.AuctionEntity>();
    public DbSet<BidEntity> Bids => Set<BidEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuctionDbContext).Assembly);
        modelBuilder.Entity<BidEntity>()
            .HasOne(b => b.Auction)
            .WithMany(a => a.Bids)
            .HasForeignKey(b => b.AuctionId)
            .OnDelete(DeleteBehavior.Cascade);
        base.OnModelCreating(modelBuilder);
    }
}

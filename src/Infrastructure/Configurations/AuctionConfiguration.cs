using Auction.Infrastructure.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auction.Infrastructure.Configurations;

public class AuctionConfiguration : IEntityTypeConfiguration<AuctionEntity>
{
    public void Configure(EntityTypeBuilder<AuctionEntity> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.VehicleVin).IsRequired();
        builder.Property(a => a.Status).IsRequired();
        builder.Property(a => a.StartedAt).IsRequired();
        builder.HasOne(a => a.Vehicle)
            .WithMany()
            .HasForeignKey(a => a.VehicleVin)
            .IsRequired();
        builder.HasMany(a => a.Bids)
            .WithOne()
            .HasForeignKey(b => b.AuctionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class BidConfiguration : IEntityTypeConfiguration<BidEntity>
{
    public void Configure(EntityTypeBuilder<BidEntity> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Amount).IsRequired();
        builder.Property(b => b.PlacedAt).IsRequired();
        builder.Property(b => b.Bidder).IsRequired();
    }
}

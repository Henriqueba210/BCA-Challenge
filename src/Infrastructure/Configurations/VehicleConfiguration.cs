using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infrastructure.Common;

namespace Infrastructure.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<VehicleEntity>
{
    public void Configure(EntityTypeBuilder<VehicleEntity> builder)
    {
        builder.HasKey(v => v.Vin);
        builder.HasDiscriminator<string>("VehicleType")
            .HasValue<SedanEntity>("Sedan")
            .HasValue<HatchbackEntity>("Hatchback")
            .HasValue<SUVEntity>("SUV")
            .HasValue<TruckEntity>("Truck");
        builder.Property(v => v.Manufacturer).IsRequired();
        builder.Property(v => v.Model).IsRequired();
        builder.Property(v => v.Year).IsRequired();
        builder.Property(v => v.StartingBid).IsRequired();
    }
}

public class SedanConfiguration : IEntityTypeConfiguration<SedanEntity>
{
    public void Configure(EntityTypeBuilder<SedanEntity> builder)
    {
        builder.Property(s => s.NumberOfDoors).IsRequired();
    }
}

public class HatchbackConfiguration : IEntityTypeConfiguration<HatchbackEntity>
{
    public void Configure(EntityTypeBuilder<HatchbackEntity> builder)
    {
        builder.Property(h => h.NumberOfDoors).IsRequired();
    }
}

public class SUVConfiguration : IEntityTypeConfiguration<SUVEntity>
{
    public void Configure(EntityTypeBuilder<SUVEntity> builder)
    {
        builder.Property(s => s.NumberOfSeats).IsRequired();
    }
}

public class TruckConfiguration : IEntityTypeConfiguration<TruckEntity>
{
    public void Configure(EntityTypeBuilder<TruckEntity> builder)
    {
        builder.Property(t => t.LoadCapacity).IsRequired();
    }
}

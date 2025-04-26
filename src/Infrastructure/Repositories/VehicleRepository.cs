using Auction.Application.Features.Vehicle.Abstractions;
using Auction.Domain.Entities;
using Auction.Infrastructure.Models;

using Mapster;

using MapsterMapper;

using Microsoft.EntityFrameworkCore;

namespace Auction.Infrastructure.Repositories;

public class VehicleRepository(AuctionDbContext context, IMapper mapper) : IVehicleRepository
{
    private readonly AuctionDbContext _context = context;

    public async Task<BaseVehicle?> GetByVinAsync(string vin, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Set<VehicleEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Vin.ToLower() == vin.ToLower(), cancellationToken);

        return entity is null ? null : mapper.Map<BaseVehicle>(entity);
    }

    public async Task<BaseVehicle> AddAsync(BaseVehicle vehicle, CancellationToken cancellationToken = default)
    {
        VehicleEntity entity = vehicle switch
        {
            Sedan s => mapper.Map<SedanEntity>(s),
            Hatchback h => mapper.Map<HatchbackEntity>(h),
            Suv suv => mapper.Map<SuvEntity>(suv),
            Truck t => mapper.Map<TruckEntity>(t),
            _ => throw new NotSupportedException()
        };

        await _context.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return mapper.Map<BaseVehicle>(entity);
    }

    public async Task<List<BaseVehicle>> SearchAsync(string? type, string? manufacturer, string? model, int? year, CancellationToken cancellationToken = default)
    {
        IQueryable<VehicleEntity> query = _context.Set<VehicleEntity>();

        if (!string.IsNullOrEmpty(type))
        {
            query = type switch
            {
                "Sedan" => query.OfType<SedanEntity>(),
                "Hatchback" => query.OfType<HatchbackEntity>(),
                "SUV" => query.OfType<SuvEntity>(),
                "Truck" => query.OfType<TruckEntity>(),
                _ => query
            };
        }
        if (!string.IsNullOrEmpty(manufacturer))
            query = query.Where(v => v.Manufacturer == manufacturer);
        if (!string.IsNullOrEmpty(model))
            query = query.Where(v => v.Model == model);
        if (year.HasValue)
            query = query.Where(v => v.Year == year);

        var entities = await query.ToListAsync(cancellationToken);

        var vehicles = entities.Select(e => mapper.Map<BaseVehicle>(e)).ToList();

        return vehicles;
    }
}

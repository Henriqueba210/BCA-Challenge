
using Auction.Application.Features.Vehicle.Abstractions;
using Auction.Domain.Entities;

using Mapster;

using Microsoft.EntityFrameworkCore;

namespace Auction.Infrastructure.Repositories;

public class VehicleRepository(AuctionDbContext context) : IVehicleRepository
{
    private readonly AuctionDbContext _context = context;

    public async Task<BaseVehicle?> GetByVinAsync(string vin, CancellationToken cancellationToken)
    {
        var entity = await _context.Vehicles.FindAsync(new object[] { vin }, cancellationToken);
        return entity?.Adapt<BaseVehicle>();
    }

    public async Task AddAsync(BaseVehicle vehicle, CancellationToken cancellationToken = default)
    {
        var entity = vehicle.Adapt<Entities.VehicleEntity>();
        await _context.Vehicles.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<BaseVehicle>> SearchAsync(string? type, string? manufacturer, string? model, int? year, CancellationToken cancellationToken)
    {
        var query = _context.Vehicles.AsQueryable();
        if (!string.IsNullOrEmpty(type))
            query = query.Where(v => v.Type.ToString() == type);
        if (!string.IsNullOrEmpty(manufacturer))
            query = query.Where(v => v.Manufacturer == manufacturer);
        if (!string.IsNullOrEmpty(model))
            query = query.Where(v => v.Model == model);
        if (year.HasValue)
            query = query.Where(v => v.Year == year);
        var entities = await query.ToListAsync(cancellationToken);
        return entities.Adapt<List<BaseVehicle>>();
    }
}

using Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly AuctionDbContext _context;
    public VehicleRepository(AuctionDbContext context) => _context = context;

    public async Task<Vehicle?> GetByVinAsync(string vin, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Vehicles.FindAsync(new object[] { vin }, cancellationToken);
        return entity?.Adapt<Vehicle>();
    }

    public async Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
    {
        var entity = vehicle.Adapt<Infrastructure.Entities.VehicleEntity>();
        await _context.Vehicles.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Vehicle>> SearchAsync(string? type, string? manufacturer, string? model, int? year, CancellationToken cancellationToken = default)
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
        return entities.Adapt<List<Vehicle>>();
    }
}

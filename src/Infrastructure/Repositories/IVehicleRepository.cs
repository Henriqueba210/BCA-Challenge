using Domain.Entities;

namespace Infrastructure.Repositories;

public interface IVehicleRepository
{
    Task<Vehicle?> GetByVinAsync(string vin, CancellationToken cancellationToken = default);
    Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default);
    Task<List<Vehicle>> SearchAsync(string? type, string? manufacturer, string? model, int? year, CancellationToken cancellationToken = default);
}

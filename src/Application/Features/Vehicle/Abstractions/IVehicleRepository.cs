using Auction.Domain.Entities;

namespace Auction.Application.Features.Vehicle.Abstractions;

public interface IVehicleRepository
{
    Task<BaseVehicle?> GetByVinAsync(string vin, CancellationToken cancellationToken = default);
    Task AddAsync(BaseVehicle vehicle, CancellationToken cancellationToken = default);
    Task<List<BaseVehicle>> SearchAsync(string? type, string? manufacturer, string? model, int? year, CancellationToken cancellationToken = default);
}

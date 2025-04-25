using Domain.Entities;

namespace Infrastructure.Repositories;

public interface IAuctionRepository
{
    Task<Auction?> GetByVehicleVinAsync(string vin, CancellationToken cancellationToken = default);
    Task<Auction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Auction auction, CancellationToken cancellationToken = default);
    Task UpdateAsync(Auction auction, CancellationToken cancellationToken = default);
}

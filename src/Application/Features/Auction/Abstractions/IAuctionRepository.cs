
using Auction.Domain.Entities;

namespace Auction.Application.Features.Auction.Abstractions;

public interface IAuctionRepository
{
    Task<List<AuctionEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AuctionEntity?> GetByVehicleVinAsync(string vin, CancellationToken cancellationToken = default);
    Task<AuctionEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(AuctionEntity auction, CancellationToken cancellationToken = default);
    Task UpdateAsync(AuctionEntity auction, CancellationToken cancellationToken = default);
}

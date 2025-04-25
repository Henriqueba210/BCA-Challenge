using Auction.Domain.Entities;

namespace Auction.Application.Features.Auction.Abstractions;

public interface IAuctionRepository
{
    Task<List<AuctionEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AuctionEntity?> GetByVehicleVinAsync(string vin, CancellationToken cancellationToken = default);
    Task<AuctionEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AuctionEntity> AddAsync(AuctionEntity auction, CancellationToken cancellationToken = default);
    Task<AuctionEntity> UpdateAsync(AuctionEntity? auction = null, Bid? newBid = null, CancellationToken cancellationToken = default);
}

using Auction.Application.Features.Auction.Abstractions;
using Auction.Domain.Entities;

using Mapster;

using Microsoft.EntityFrameworkCore;

namespace Auction.Infrastructure.Repositories;

public class AuctionRepository : IAuctionRepository
{
    private readonly AuctionDbContext _context;
    public AuctionRepository(AuctionDbContext context) => _context = context;

    public async Task<AuctionEntity?> GetByVehicleVinAsync(string vin, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Auctions.Include(a => a.Bids).FirstOrDefaultAsync(a => a.VehicleVin == vin, cancellationToken);
        return entity?.Adapt<AuctionEntity>();
    }

    public async Task<AuctionEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Auctions.Include(a => a.Bids).FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        return entity?.Adapt<AuctionEntity>();
    }

    public async Task AddAsync(AuctionEntity auction, CancellationToken cancellationToken = default)
    {
        var entity = auction.Adapt<Models.AuctionEntity>();
        await _context.Auctions.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(AuctionEntity auction, CancellationToken cancellationToken = default)
    {
        var entity = auction.Adapt<Models.AuctionEntity>();
        _context.Auctions.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<AuctionEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.Auctions.Include(a => a.Bids).ToListAsync(cancellationToken);
        return entities.Adapt<List<AuctionEntity>>();
    }
}

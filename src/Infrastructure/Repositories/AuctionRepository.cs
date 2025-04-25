using Domain.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AuctionRepository : IAuctionRepository
{
    private readonly AuctionDbContext _context;
    public AuctionRepository(AuctionDbContext context) => _context = context;

    public async Task<Auction?> GetByVehicleVinAsync(string vin, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Auctions.Include(a => a.Bids).FirstOrDefaultAsync(a => a.VehicleVin == vin, cancellationToken);
        return entity?.Adapt<Auction>();
    }

    public async Task<Auction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Auctions.Include(a => a.Bids).FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        return entity?.Adapt<Auction>();
    }

    public async Task AddAsync(Auction auction, CancellationToken cancellationToken = default)
    {
        var entity = auction.Adapt<Infrastructure.Entities.AuctionEntity>();
        await _context.Auctions.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Auction auction, CancellationToken cancellationToken = default)
    {
        var entity = auction.Adapt<Infrastructure.Entities.AuctionEntity>();
        _context.Auctions.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

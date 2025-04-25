using System.Collections.Concurrent;
using Auction.Application.Features.Auction.Abstractions;
using Auction.Domain.Entities;

using Mapster;

using Microsoft.EntityFrameworkCore;

namespace Auction.Infrastructure.Repositories;

public class AuctionRepository : IAuctionRepository
{
    private readonly AuctionDbContext _context;
    public AuctionRepository(AuctionDbContext context) => _context = context;

    // Static dictionary to hold semaphores per auction
    private static readonly ConcurrentDictionary<Guid, SemaphoreSlim> _auctionLocks = new();

    public async Task<AuctionEntity?> GetByVehicleVinAsync(string vin, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Auctions
            .Include(a => a.Bids)
            .Include(a => a.Vehicle)
            .FirstOrDefaultAsync(a => a.VehicleVin == vin, cancellationToken);
        return entity?.Adapt<AuctionEntity>();
    }

    public async Task<AuctionEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Auctions
            .Include(a => a.Bids)
            .Include(a => a.Vehicle)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        return entity?.Adapt<AuctionEntity>();
    }

    public async Task<AuctionEntity> AddAsync(AuctionEntity auction, CancellationToken cancellationToken = default)
    {
        var entity = auction.Adapt<Models.AuctionEntity>();
        if (entity.Bids != null)
        {
            foreach (var bid in entity.Bids)
            {
                bid.AuctionId = entity.Id;
            }
        }
        var entry = await _context.Auctions.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        await _context.Entry(entry.Entity).Collection(e => e.Bids).LoadAsync(cancellationToken);
        await _context.Entry(entry.Entity).Reference(e => e.Vehicle).LoadAsync(cancellationToken);
        return entry.Entity.Adapt<AuctionEntity>();
    }

    public async Task<AuctionEntity> UpdateAsync(AuctionEntity auction, CancellationToken cancellationToken = default)
    {
        var semaphore = _auctionLocks.GetOrAdd(auction.Id, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync(cancellationToken);
        try
        {
            var existingEntity = await _context.Auctions
                .Include(a => a.Bids)
                .Include(a => a.Vehicle)
                .FirstOrDefaultAsync(a => a.Id == auction.Id, cancellationToken)
                ?? throw new InvalidOperationException("Auction not found");

            // Use SetValues for scalar properties only
            _context.Entry(existingEntity).CurrentValues.SetValues(auction.Adapt<Models.AuctionEntity>());

            // Sync bids as before
            var incomingBids = auction.Bids.Select(b => b.Id).ToHashSet();
            existingEntity.Bids.RemoveAll(b => !incomingBids.Contains(b.Id));
            foreach (var bid in auction.Bids)
            {
                var existingBid = existingEntity.Bids.FirstOrDefault(b => b.Id == bid.Id);
                if (existingBid == null)
                {
                    var newBid = bid.Adapt<Models.BidEntity>();
                    newBid.AuctionId = existingEntity.Id;
                    existingEntity.Bids.Add(newBid);
                    _context.Entry(newBid).State = EntityState.Added;
                }
                else
                {
                    existingBid.Amount = bid.Amount;
                    existingBid.PlacedAt = bid.PlacedAt;
                    existingBid.Bidder = bid.Bidder;
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            await _context.Entry(existingEntity).Collection(e => e.Bids).LoadAsync(cancellationToken);
            await _context.Entry(existingEntity).Reference(e => e.Vehicle).LoadAsync(cancellationToken);

            return existingEntity.Adapt<AuctionEntity>();
        }
        finally
        {
            semaphore.Release();
        }
    }

    public async Task<List<AuctionEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.Auctions.Include(a => a.Bids).ToListAsync(cancellationToken);
        return entities.Adapt<List<AuctionEntity>>();
    }
}

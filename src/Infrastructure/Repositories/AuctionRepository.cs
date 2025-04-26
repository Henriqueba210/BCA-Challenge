using System.Collections.Concurrent;
using Auction.Application.Features.Auction.Abstractions;
using Auction.Domain.Entities;

using Mapster;

using Microsoft.EntityFrameworkCore;

namespace Auction.Infrastructure.Repositories;

internal static class AuctionRepositoryLocks
{
    public static readonly ConcurrentDictionary<Guid, SemaphoreSlim> AuctionLocks = new();
}

public class AuctionRepository(AuctionDbContext context) : IAuctionRepository
{
    public async Task<AuctionEntity?> GetByVehicleVinAsync(string vin, CancellationToken cancellationToken = default)
    {
        var entity = await context.Auctions
            .Include(a => a.Bids)
            .Include(a => a.Vehicle)
            .FirstOrDefaultAsync(a => a.VehicleVin == vin, cancellationToken);
        return entity?.Adapt<AuctionEntity>();
    }

    public async Task<AuctionEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Auctions
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
        var entry = await context.Auctions.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        await context.Entry(entry.Entity).Collection(e => e.Bids).LoadAsync(cancellationToken);
        await context.Entry(entry.Entity).Reference(e => e.Vehicle).LoadAsync(cancellationToken);
        return entry.Entity.Adapt<AuctionEntity>();
    }

    public async Task<AuctionEntity> UpdateAsync(AuctionEntity? auction = null, Bid? newBid = null, CancellationToken cancellationToken = default)
    {
        var auctionId = auction?.Id ?? newBid?.AuctionId
            ?? throw new ArgumentException("Either auction or newBid with AuctionId must be provided.");

        var semaphore = AuctionRepositoryLocks.AuctionLocks.GetOrAdd(auctionId, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync(cancellationToken);
        try
        {
            var existingEntity = await context.Auctions
                .Include(a => a.Bids)
                .Include(a => a.Vehicle)
                .FirstOrDefaultAsync(a => a.Id == auctionId, cancellationToken)
                ?? throw new InvalidOperationException("Auction not found");

            if (existingEntity.Status != (int)Domain.Enums.AuctionStatus.Active)
                throw new InvalidOperationException("Auction is not active");

            if (newBid != null)
            {
                var highestBid = existingEntity.Bids.OrderByDescending(b => b.Amount).FirstOrDefault();
                if (highestBid != null && newBid.Amount <= highestBid.Amount)
                    throw new InvalidOperationException("Bid amount must be higher than the current highest bid");

                var bidEntity = newBid.Adapt<Models.BidEntity>();
                bidEntity.AuctionId = existingEntity.Id;
                existingEntity.Bids.Add(bidEntity);
                context.Entry(bidEntity).State = EntityState.Added;
            }

            if (auction != null)
            {
                context.Entry(existingEntity).CurrentValues.SetValues(auction.Adapt<Models.AuctionEntity>());
            }

            await context.SaveChangesAsync(cancellationToken);
            await context.Entry(existingEntity).Collection(e => e.Bids).LoadAsync(cancellationToken);
            await context.Entry(existingEntity).Reference(e => e.Vehicle).LoadAsync(cancellationToken);

            return existingEntity.Adapt<AuctionEntity>();
        }
        finally
        {
            semaphore.Release();
        }
    }

    public async Task<List<AuctionEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await context.Auctions.Include(a => a.Bids).ToListAsync(cancellationToken);
        return entities.Adapt<List<AuctionEntity>>();
    }
}

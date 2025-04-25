using Auction.Domain.Entities;
using Auction.Domain.Enums;
using Auction.Infrastructure;
using Auction.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;

using Shouldly;

namespace Infrastructure.Test.Repositories;

public class AuctionRepositoryConcurrencyTests
{
    private AuctionDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AuctionDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AuctionDbContext(options);
    }

    [Fact]
    public async Task UpdateAsync_ShouldEnforceSemaphore_And_ValidateBids()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var repo = new AuctionRepository(context);

        var vin = "CONCURVININFRA";
        var vehicle = new Sedan
        {
            Vin = vin,
            Manufacturer = "Test",
            Model = "TestModel",
            Year = 2020,
            NumberOfDoors = 4,
            StartingBid = 1000
        };

        var auction = new AuctionEntity
        {
            Id = Guid.NewGuid(),
            VehicleVin = vin,
            Vehicle = vehicle,
            Status = AuctionStatus.Active,
            StartedAt = DateTime.UtcNow,
            Bids = new List<Bid>()
        };
        await repo.AddAsync(auction);

        // Prepare bids
        var bidAmounts = new[] { 1000m, 2000m, 1500m, 2500m, 2400m };
        var results = new List<(decimal amount, bool success)>();
        foreach (var amount in bidAmounts)
        {
            var bid = new Bid
            {
                Id = Guid.NewGuid(),
                AuctionId = auction.Id,
                Amount = amount,
                PlacedAt = DateTime.UtcNow,
                Bidder = $"user-{amount}"
            };
            try
            {
                await repo.UpdateAsync(null, bid);
                results.Add((amount, true));
            }
            catch (InvalidOperationException)
            {
                results.Add((amount, false));
            }
            await Task.Delay(5);
        }

        // Only bids that are strictly higher than the previous highest should succeed
        var successfulBids = results.Where(r => r.success).Select(r => r.amount).OrderBy(a => a).ToList();
        successfulBids.ShouldBe(new List<decimal> { 1000m, 2000m, 2500m });

        // Auction should be left with only the valid bids, in order
        var finalAuction = await repo.GetByIdAsync(auction.Id);
        finalAuction.ShouldNotBeNull();
        finalAuction.Bids.Select(b => b.Amount).OrderBy(a => a).ToList().ShouldBe(successfulBids);

        // Auction should still be active
        finalAuction.Status.ShouldBe(AuctionStatus.Active);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrow_WhenAuctionNotActive()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);
        var repo = new AuctionRepository(context);

        var vin = "INACTIVEVIN";
        var vehicle = new Sedan
        {
            Vin = vin,
            Manufacturer = "Test",
            Model = "TestModel",
            Year = 2020,
            NumberOfDoors = 4,
            StartingBid = 1000
        };

        var auction = new AuctionEntity
        {
            Id = Guid.NewGuid(),
            VehicleVin = vin,
            Vehicle = vehicle,
            Status = AuctionStatus.Closed,
            StartedAt = DateTime.UtcNow,
            Bids = new List<Bid>()
        };
        await repo.AddAsync(auction);

        var loaded = await repo.GetByIdAsync(auction.Id);
        loaded.Bids.Add(new Bid
        {
            Id = Guid.NewGuid(),
            AuctionId = auction.Id,
            Amount = 1000m,
            PlacedAt = DateTime.UtcNow,
            Bidder = "user"
        });

        await Should.ThrowAsync<InvalidOperationException>(async () =>
        {
            await repo.UpdateAsync(loaded);
        });
    }
}

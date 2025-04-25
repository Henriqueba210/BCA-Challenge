using Application.Features.Auction.Commands;

using Ardalis.Result;

using Auction.Application.Features.Auction.Abstractions;
using Auction.Domain.Entities;

using Domain.Enums;
using MediatR;

namespace Auction.Application.Features.Auction.Commands;

public class PlaceBidCommandHandler(IAuctionRepository auctionRepository) : IRequestHandler<PlaceBidCommand, Result>
{
    public async Task<Result> Handle(PlaceBidCommand request, CancellationToken cancellationToken)
    {
        var auction = await auctionRepository.GetByVehicleVinAsync(request.Vin, cancellationToken);
        if (auction is null)
            return Result.Error($"No auction found for vehicle VIN '{request.Vin}'.");
        if (auction.Status != AuctionStatus.Active)
            return Result.Error("Auction is not active. Cannot place bid.");
        var highestBid = auction.Bids.OrderByDescending(b => b.Amount).FirstOrDefault();
        if (highestBid != null && request.Amount <= highestBid.Amount)
            return Result.Error($"Bid must be greater than the current highest bid ({highestBid.Amount:C}).");
        var bid = new Bid
        {
            Id = Guid.NewGuid(),
            AuctionId = auction.Id,
            Amount = request.Amount,
            PlacedAt = DateTime.UtcNow,
            Bidder = request.Bidder
        };
        auction.Bids.Add(bid);
        await auctionRepository.UpdateAsync(auction, cancellationToken);
        return Result.Success();
    }
}

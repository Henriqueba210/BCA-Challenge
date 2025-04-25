using Ardalis.Result;

using Auction.Application.Features.Auction.Abstractions;
using Auction.Application.Features.Auction.Commands.Command;
using Auction.Application.Features.Auction.Common;
using Auction.Domain.Entities;
using Auction.Domain.Enums;

using Mapster;

using MediatR;

namespace Auction.Application.Features.Auction.Commands.Handler;

public class PlaceBidCommandHandler(IAuctionRepository auctionRepository) : IRequestHandler<PlaceBidCommand, Result<AuctionDto>>
{
    public async Task<Result<AuctionDto>> Handle(PlaceBidCommand request, CancellationToken cancellationToken)
    {
        var auction = await auctionRepository.GetByVehicleVinAsync(request.Vin, cancellationToken);
        
        if (auction is null)
            return Result.NotFound("Auction does not exist");
        
        if (auction!.Status != AuctionStatus.Active)
            return Result.Invalid(new ValidationError("Auction is not active"));
        var highestBid = auction.Bids.OrderByDescending(b => b.Amount).FirstOrDefault();
        if (highestBid != null && request.Amount <= highestBid.Amount)
            return Result.Invalid(new ValidationError("Bid amount must be higher than the current highest bid"));
        var bid = new Bid
        {
            Id = Guid.NewGuid(),
            AuctionId = auction.Id,
            Amount = request.Amount,
            PlacedAt = DateTime.UtcNow,
            Bidder = request.Bidder
        };
        auction.Bids.Add(bid);
        var updatedAuction = await auctionRepository.UpdateAsync(auction, cancellationToken);
        return Result.Success(updatedAuction.Adapt<AuctionDto>());
    }
}

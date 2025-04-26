using Ardalis.Result;
using Auction.Application.Features.Auction.Abstractions;
using Auction.Application.Features.Auction.Commands.Command;
using Auction.Application.Features.Auction.Common;
using Auction.Domain.Entities;
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

        var bid = new Bid
        {
            Id = Guid.NewGuid(),
            AuctionId = auction.Id,
            Amount = request.Amount,
            PlacedAt = DateTime.UtcNow,
            Bidder = request.Bidder
        };

        var updatedAuction = await auctionRepository.UpdateAsync(null, bid, cancellationToken);
        return Result.Success(updatedAuction.Adapt<AuctionDto>());
    }
}

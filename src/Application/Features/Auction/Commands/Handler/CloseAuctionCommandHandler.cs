using Ardalis.Result;

using Auction.Application.Features.Auction.Abstractions;
using Auction.Application.Features.Auction.Commands.Command;
using Auction.Application.Features.Auction.Common;
using Auction.Domain.Enums;

using Mapster;

using MediatR;

namespace Auction.Application.Features.Auction.Commands.Handler;

public class CloseAuctionCommandHandler(IAuctionRepository auctionRepository) : IRequestHandler<CloseAuctionCommand, Result<AuctionDto>>
{
    public async Task<Result<AuctionDto>> Handle(CloseAuctionCommand request, CancellationToken cancellationToken)
    {
        var auction = await auctionRepository.GetByVehicleVinAsync(request.Vin, cancellationToken);
        if (auction == null)
            return Result.NotFound("Auction does not exist.");
        if (auction.Status != AuctionStatus.Active)
            return Result.Invalid(new ValidationError("Auction is not active and cannot be closed."));
        auction.Status = AuctionStatus.Closed;
        auction.ClosedAt = DateTime.UtcNow;
        var updatedAuction = await auctionRepository.UpdateAsync(auction, cancellationToken);
        return Result.Success(updatedAuction.Adapt<AuctionDto>());
    }
}

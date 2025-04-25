using Ardalis.Result;

using Auction.Application.Features.Auction.Abstractions;

using Domain.Enums;

using MediatR;

namespace Auction.Application.Features.Auction.Commands;

public class CloseAuctionCommandHandler(IAuctionRepository auctionRepository) : IRequestHandler<CloseAuctionCommand, Result>
{
    public async Task<Result> Handle(CloseAuctionCommand request, CancellationToken cancellationToken)
    {
        var auction = await auctionRepository.GetByVehicleVinAsync(request.Vin, cancellationToken);
        if (auction is null)
            return Result.Error($"No auction found for vehicle VIN '{request.Vin}'.");
        if (auction.Status != AuctionStatus.Active)
            return Result.Error("Auction is not active and cannot be closed.");
        auction.Status = AuctionStatus.Closed;
        auction.ClosedAt = DateTime.UtcNow;
        await auctionRepository.UpdateAsync(auction, cancellationToken);
        return Result.Success();
    }
}

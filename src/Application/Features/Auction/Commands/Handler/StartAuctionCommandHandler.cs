using Ardalis.Result;

using Auction.Application.Features.Auction.Abstractions;
using Auction.Application.Features.Auction.Commands.Command;
using Auction.Application.Features.Auction.Common;
using Auction.Domain.Enums;

using Mapster;

using MediatR;

namespace Auction.Application.Features.Auction.Commands.Handler;

public class StartAuctionCommandHandler(IAuctionRepository auctionRepository) : IRequestHandler<StartAuctionCommand, Result<AuctionDto>>
{
    public async Task<Result<AuctionDto>> Handle(StartAuctionCommand request, CancellationToken cancellationToken)
    {
        var existingAuction = await auctionRepository.GetByVehicleVinAsync(request.Vin, cancellationToken);
        if (existingAuction is not null && existingAuction.Status == AuctionStatus.Active)
            return Result.Invalid(new ValidationError("An active auction already exists for this vehicle."));

        var auction = new Domain.Entities.AuctionEntity
        {
            Id = Guid.NewGuid(),
            VehicleVin = request.Vin,
            Status = AuctionStatus.Active,
            StartedAt = DateTime.UtcNow,
            Bids = []
        };
        var addedAuction = await auctionRepository.AddAsync(auction, cancellationToken);
        var auctionDto = addedAuction.Adapt<AuctionDto>();
        return Result.Success(auctionDto);
    }
}

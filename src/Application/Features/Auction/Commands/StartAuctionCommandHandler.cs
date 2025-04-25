using Ardalis.Result;

using Auction.Application.Features.Auction.Abstractions;
using Auction.Application.Features.Vehicle.Abstractions;

using Domain.Enums;
using Domain.ValueObjects;

using MediatR;

namespace Auction.Application.Features.Auction.Commands;

public class StartAuctionCommandHandler(IVehicleRepository vehicleRepository, IAuctionRepository auctionRepository) : IRequestHandler<StartAuctionCommand, Result>
{
    public async Task<Result> Handle(StartAuctionCommand request, CancellationToken cancellationToken)
    {
        var vehicle = await vehicleRepository.GetByVinAsync(request.Vin, cancellationToken);
        if (vehicle is null)
            return Result.Error($"Vehicle with VIN '{request.Vin}' does not exist.");

        var existingAuction = await auctionRepository.GetByVehicleVinAsync(request.Vin, cancellationToken);
        if (existingAuction is not null && existingAuction.Status == AuctionStatus.Active)
            return Result.Error("An active auction already exists for this vehicle.");

        var auction = new Domain.Entities.AuctionEntity
        {
            Id = Guid.NewGuid(),
            VehicleVin = new Vin(request.Vin),
            Status = AuctionStatus.Active,
            StartedAt = DateTime.UtcNow,
            Bids = []
        };
        await auctionRepository.AddAsync(auction, cancellationToken);
        return Result.Success();
    }
}

using Ardalis.Result;

using Auction.Application.Features.Auction.Abstractions;
using Auction.Application.Features.Auction.Common;
using Auction.Application.Features.Auction.Queries.Query;
using Auction.Application.Features.Vehicle.Abstractions;
using Auction.Application.Features.Vehicle.Common;

using Mapster;

using MediatR;

namespace Auction.Application.Features.Auction.Queries.Handler;

public class GetAuctionByIdQueryHandler(
    IAuctionRepository auctionRepository,
    IVehicleRepository vehicleRepository
) : IRequestHandler<GetAuctionByIdQuery, Result<AuctionDto>>
{
    public async Task<Result<AuctionDto>> Handle(GetAuctionByIdQuery request, CancellationToken cancellationToken)
    {
        var auction = await auctionRepository.GetByIdAsync(request.AuctionId, cancellationToken);
        if (auction is null)
            return Result.NotFound();
        var vehicle = await vehicleRepository.GetByVinAsync(auction.VehicleVin, cancellationToken);
        if (vehicle is null)
            return Result.Error($"Vehicle with VIN '{auction.VehicleVin}' not found for auction.");
        var auctionDto = auction.Adapt<AuctionDto>();
        auctionDto.Vehicle = vehicle.Adapt<VehicleDto>();
        return Result.Success(auctionDto);
    }
}

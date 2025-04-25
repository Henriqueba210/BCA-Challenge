using Ardalis.Result;

using Auction.Application.Features.Auction.Abstractions;
using Auction.Application.Features.Auction.Common;
using Auction.Application.Features.Auction.Queries.Query;
using Auction.Application.Features.Vehicle.Abstractions;
using Auction.Application.Features.Vehicle.Common;

using Mapster;

using MediatR;

namespace Auction.Application.Features.Auction.Queries.Handler;

public class GetAuctionByVinQueryHandler(
    IAuctionRepository auctionRepository,
    IVehicleRepository vehicleRepository
) : IRequestHandler<GetAuctionByVinQuery, Result<AuctionDto>>
{
    public async Task<Result<AuctionDto>> Handle(GetAuctionByVinQuery request, CancellationToken cancellationToken)
    {
        var auction = await auctionRepository.GetByVehicleVinAsync(request.Vin, cancellationToken);
        if (auction is null)
            return Result.NotFound();
        var vehicle = await vehicleRepository.GetByVinAsync(request.Vin, cancellationToken);
        if (vehicle is null)
            return Result.Error($"Vehicle with VIN '{request.Vin}' not found for auction.");
        var auctionDto = auction.Adapt<AuctionDto>();
        auctionDto.Vehicle = vehicle.Adapt<VehicleDto>();
        return Result.Success(auctionDto);
    }
}

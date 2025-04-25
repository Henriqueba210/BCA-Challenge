using Application.Features.Auction.Queries;

using Ardalis.Result;

using Auction.Application.Features.Auction.Abstractions;
using Auction.Application.Features.Auction.Common;
using Auction.Application.Features.Vehicle.Abstractions;

using Mapster;
using MediatR;

namespace Auction.Application.Features.Auction.Queries;

public class ListAuctionsQueryHandler(
    IAuctionRepository auctionRepository,
    IVehicleRepository vehicleRepository
) : IRequestHandler<ListAuctionsQuery, Result<List<AuctionDto>>>
{
    public async Task<Result<List<AuctionDto>>> Handle(ListAuctionsQuery request, CancellationToken cancellationToken)
    {
        // For demonstration, assume auctionRepository has a method to get all auctions. If not, you may need to implement it.
        var allAuctions = await auctionRepository.GetAllAsync(cancellationToken);
        var filtered = allAuctions.AsQueryable();
        if (!string.IsNullOrEmpty(request.Status))
            filtered = filtered.Where(a => a.Status.ToString() == request.Status);
        if (!string.IsNullOrEmpty(request.VehicleType))
            filtered = filtered.Where(a => a.VehicleVin.Value != null); // You may want to filter by vehicle type if you have that info
        var auctionList = filtered.ToList();
        var result = new List<AuctionDto>();
        foreach (var auction in auctionList)
        {
            var vehicle = await vehicleRepository.GetByVinAsync(auction.VehicleVin.Value, cancellationToken);
            var auctionDto = auction.Adapt<AuctionDto>();
            auctionDto.Vehicle = vehicle?.Adapt<VehicleDto>() ?? new VehicleDto { Vin = auction.VehicleVin.Value };
            result.Add(auctionDto);
        }
        return Result.Success(result);
    }
}

using Ardalis.Result;

using Auction.Application.Features.Auction.Abstractions;
using Auction.Application.Features.Auction.Common;
using Auction.Application.Features.Auction.Queries.Query;
using Auction.Application.Features.Vehicle.Abstractions;
using Auction.Application.Features.Vehicle.Common;

using Mapster;

using MediatR;

namespace Auction.Application.Features.Auction.Queries.Handler;

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
        if (!string.IsNullOrEmpty(request.VehicleType) || !string.IsNullOrEmpty(request.Manufacturer) || !string.IsNullOrEmpty(request.Model) || request.Year.HasValue)
        {
            // To filter by vehicle properties, fetch all relevant vehicles and join
            var vehicles = await vehicleRepository.SearchAsync(request.VehicleType, request.Manufacturer, request.Model, request.Year, cancellationToken);
            var vins = vehicles.Select(v => v.Vin).ToHashSet();
            filtered = filtered.Where(a => vins.Contains(a.VehicleVin));
        }
        var auctionList = filtered.ToList();
        var result = new List<AuctionDto>();
        foreach (var auction in auctionList)
        {
            var vehicle = await vehicleRepository.GetByVinAsync(auction.VehicleVin, cancellationToken);
            var auctionDto = auction.Adapt<AuctionDto>();
            auctionDto.Vehicle = vehicle?.Adapt<VehicleDto>() ?? new VehicleDto { Vin = auction.VehicleVin };
            result.Add(auctionDto);
        }
        return Result.Success(result);
    }
}

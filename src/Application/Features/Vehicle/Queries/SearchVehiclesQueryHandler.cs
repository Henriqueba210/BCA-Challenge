using Auction.Application.Features.Vehicle.Common;
using Auction.Application.Features.Vehicle.Abstractions;
using Ardalis.Result;
using MediatR;
using Mapster;

namespace Auction.Application.Features.Vehicle.Queries;

public class SearchVehiclesQueryHandler(IVehicleRepository vehicleRepository) : IRequestHandler<SearchVehiclesQuery, Result<List<VehicleDto>>>
{
    public async Task<Result<List<VehicleDto>>> Handle(SearchVehiclesQuery request, CancellationToken cancellationToken)
    {
        var vehicles = await vehicleRepository.SearchAsync(request.Type, request.Manufacturer, request.Model, request.Year, cancellationToken);
        var dtos = vehicles.Adapt<List<VehicleDto>>();
        return Result.Success(dtos);
    }
}

using Ardalis.Result;

using Auction.Application.Features.Vehicle.Abstractions;
using Auction.Application.Features.Vehicle.Common;
using Auction.Application.Features.Vehicle.Queries.Query;

using Mapster;

using MediatR;

namespace Auction.Application.Features.Vehicle.Queries.Handler;

public class SearchVehiclesQueryHandler(IVehicleRepository vehicleRepository) : IRequestHandler<SearchVehiclesQuery, Result<List<VehicleDto>>>
{
    public async Task<Result<List<VehicleDto>>> Handle(SearchVehiclesQuery request, CancellationToken cancellationToken)
    {
        var vehicles = await vehicleRepository.SearchAsync(request.Type, request.Manufacturer, request.Model, request.Year, cancellationToken);
        var dtos = vehicles.Adapt<List<VehicleDto>>();
        return Result.Success(dtos);
    }
}

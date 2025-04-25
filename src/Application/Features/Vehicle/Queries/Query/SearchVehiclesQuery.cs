using Ardalis.Result;

using Auction.Application.Features.Vehicle.Common;

using MediatR;

namespace Auction.Application.Features.Vehicle.Queries.Query;

public record SearchVehiclesQuery(
    string? Type,
    string? Manufacturer,
    string? Model,
    int? Year
) : IRequest<Result<List<VehicleDto>>>;

using Ardalis.Result;
using MediatR;
using System.Collections.Generic;
using Auction.Application.Features.Vehicle.Common;

namespace Auction.Application.Features.Vehicle.Queries;

public record SearchVehiclesQuery(
    string? Type,
    string? Manufacturer,
    string? Model,
    int? Year
) : IRequest<Result<List<VehicleDto>>>;

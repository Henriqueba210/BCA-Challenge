using Ardalis.Result;
using MediatR;
using System.Collections.Generic;

namespace Application.Features.Vehicle.Queries;

public record SearchVehiclesQuery(
    string? Type,
    string? Manufacturer,
    string? Model,
    int? Year
) : IRequest<Result<List<Domain.Entities.Vehicle>>>;

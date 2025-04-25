using Ardalis.Result;

using Auction.Application.Features.Vehicle.Common;

using MediatR;

namespace Auction.Application.Features.Vehicle.Commands.Command;

public record AddVehicleCommand(
    string Vin,
    string Type,
    string Manufacturer,
    string Model,
    int Year,
    decimal StartingBid,
    int? NumberOfDoors = null,
    int? NumberOfSeats = null,
    double? LoadCapacity = null
) : IRequest<Result<VehicleDto>>;

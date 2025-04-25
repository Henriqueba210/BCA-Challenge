using Ardalis.Result;
using MediatR;

namespace Application.Features.Vehicle.Commands;

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
) : IRequest<Result>;

using Auction.Application.Features.Vehicle.Abstractions;
using Ardalis.Result;
using Domain.Enums;
using MediatR;
using Auction.Domain.Entities;
using Domain.ValueObjects;

namespace Auction.Application.Features.Vehicle.Commands;

public class AddVehicleCommandHandler(IVehicleRepository vehicleRepository) : IRequestHandler<AddVehicleCommand, Result>
{
    public async Task<Result> Handle(AddVehicleCommand request, CancellationToken cancellationToken)
    {
        // Check for duplicate VIN
        var existing = await vehicleRepository.GetByVinAsync(request.Vin, cancellationToken);
        if (existing is not null)
            return Result.Error($"A vehicle with VIN '{request.Vin}' already exists.");

        BaseVehicle vehicle = request.Type switch
        {
            nameof(VehicleType.Sedan) => new Sedan
            {
                Id = new Vin(request.Vin),
                Manufacturer = request.Manufacturer,
                Model = request.Model,
                Year = request.Year,
                StartingBid = request.StartingBid,
                NumberOfDoors = request.NumberOfDoors ?? 4
            },
            nameof(VehicleType.Hatchback) => new Hatchback
            {
                Id = new Vin(request.Vin),
                Manufacturer = request.Manufacturer,
                Model = request.Model,
                Year = request.Year,
                StartingBid = request.StartingBid,
                NumberOfDoors = request.NumberOfDoors ?? 4
            },
            nameof(VehicleType.SUV) => new SUV
            {
                Id = new Vin(request.Vin),
                Manufacturer = request.Manufacturer,
                Model = request.Model,
                Year = request.Year,
                StartingBid = request.StartingBid,
                NumberOfSeats = request.NumberOfSeats ?? 5
            },
            nameof(VehicleType.Truck) => new Truck
            {
                Id = new Vin(request.Vin),
                Manufacturer = request.Manufacturer,
                Model = request.Model,
                Year = request.Year,
                StartingBid = request.StartingBid,
                LoadCapacity = request.LoadCapacity ?? 0
            },
            _ => null!
        };

        if (vehicle == null)
            return Result.Error("Invalid vehicle type.");

        await vehicleRepository.AddAsync(vehicle, cancellationToken);
        return Result.Success();
    }
}

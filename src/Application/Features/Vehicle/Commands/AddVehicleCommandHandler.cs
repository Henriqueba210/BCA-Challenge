using Application.Features.Vehicle.Abstractions;
using Ardalis.Result;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Vehicle.Commands;

public class AddVehicleCommandHandler : IRequestHandler<AddVehicleCommand, Result>
{
    private readonly IVehicleRepository _vehicleRepository;
    public AddVehicleCommandHandler(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task<Result> Handle(AddVehicleCommand request, CancellationToken cancellationToken)
    {
        // Check for duplicate VIN
        var existing = await _vehicleRepository.GetByVinAsync(request.Vin, cancellationToken);
        if (existing is not null)
            return Result.Error($"A vehicle with VIN '{request.Vin}' already exists.");

        BaseVehicle vehicle = request.Type switch
        {
            nameof(VehicleType.Sedan) => new Sedan
            {
                Id = new Domain.ValueObjects.Vin(request.Vin),
                Manufacturer = request.Manufacturer,
                Model = request.Model,
                Year = request.Year,
                StartingBid = request.StartingBid,
                NumberOfDoors = request.NumberOfDoors ?? 4
            },
            nameof(VehicleType.Hatchback) => new Hatchback
            {
                Id = new Domain.ValueObjects.Vin(request.Vin),
                Manufacturer = request.Manufacturer,
                Model = request.Model,
                Year = request.Year,
                StartingBid = request.StartingBid,
                NumberOfDoors = request.NumberOfDoors ?? 4
            },
            nameof(VehicleType.SUV) => new SUV
            {
                Id = new Domain.ValueObjects.Vin(request.Vin),
                Manufacturer = request.Manufacturer,
                Model = request.Model,
                Year = request.Year,
                StartingBid = request.StartingBid,
                NumberOfSeats = request.NumberOfSeats ?? 5
            },
            nameof(VehicleType.Truck) => new Truck
            {
                Id = new Domain.ValueObjects.Vin(request.Vin),
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

        await _vehicleRepository.AddAsync(vehicle, cancellationToken);
        return Result.Success();
    }
}

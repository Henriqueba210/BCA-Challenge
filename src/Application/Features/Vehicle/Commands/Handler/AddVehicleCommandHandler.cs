using Ardalis.Result;

using Auction.Application.Features.Vehicle.Abstractions;
using Auction.Application.Features.Vehicle.Commands.Command;
using Auction.Application.Features.Vehicle.Common;
using Auction.Domain.Entities;
using Auction.Domain.Enums;

using Mapster;

using MediatR;

namespace Auction.Application.Features.Vehicle.Commands.Handler;

public class AddVehicleCommandHandler(IVehicleRepository vehicleRepository) : IRequestHandler<AddVehicleCommand, Result<VehicleDto>>
{
    public async Task<Result<VehicleDto>> Handle(AddVehicleCommand request, CancellationToken cancellationToken)
    {
        var existing = await vehicleRepository.GetByVinAsync(request.Vin, cancellationToken);
        if (existing is not null)
            return Result.Invalid(new ValidationError($"A vehicle with VIN '{request.Vin}' already exists."));

        BaseVehicle vehicle = request.Type switch
        {
            nameof(VehicleType.Sedan) => new Sedan
            {
                Vin = request.Vin,
                Manufacturer = request.Manufacturer,
                Model = request.Model,
                Year = request.Year,
                StartingBid = request.StartingBid,
                NumberOfDoors = request.NumberOfDoors ?? 4
            },
            nameof(VehicleType.Hatchback) => new Hatchback
            {
                Vin = request.Vin,
                Manufacturer = request.Manufacturer,
                Model = request.Model,
                Year = request.Year,
                StartingBid = request.StartingBid,
                NumberOfDoors = request.NumberOfDoors ?? 4
            },
            nameof(VehicleType.SUV) => new Suv
            {
                Vin = request.Vin,
                Manufacturer = request.Manufacturer,
                Model = request.Model,
                Year = request.Year,
                StartingBid = request.StartingBid,
                NumberOfSeats = request.NumberOfSeats ?? 5
            },
            nameof(VehicleType.Truck) => new Truck
            {
                Vin = request.Vin,
                Manufacturer = request.Manufacturer,
                Model = request.Model,
                Year = request.Year,
                StartingBid = request.StartingBid,
                LoadCapacity = request.LoadCapacity ?? 0.0
            },
            _ => null!
        };

        var repositoryVehicle = await vehicleRepository.AddAsync(vehicle, cancellationToken);
        return Result.Success(repositoryVehicle.Adapt<VehicleDto>());
    }
}

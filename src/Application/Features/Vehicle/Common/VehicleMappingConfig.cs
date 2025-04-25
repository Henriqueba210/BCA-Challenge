using Auction.Domain.Entities;

using Mapster;

namespace Auction.Application.Features.Vehicle.Common;

public static class VehicleMappingConfig
{
    public static void Register()
    {
        TypeAdapterConfig<Sedan, VehicleDto>.NewConfig()
            .Map(dest => dest.Type, src => nameof(Sedan))
            .Map(dest => dest.NumberOfDoors, src => src.NumberOfDoors);

        TypeAdapterConfig<Hatchback, VehicleDto>.NewConfig()
            .Map(dest => dest.Type, src => nameof(Hatchback))
            .Map(dest => dest.NumberOfDoors, src => src.NumberOfDoors);

        TypeAdapterConfig<SUV, VehicleDto>.NewConfig()
            .Map(dest => dest.Type, src => nameof(SUV))
            .Map(dest => dest.NumberOfSeats, src => src.NumberOfSeats);

        TypeAdapterConfig<Truck, VehicleDto>.NewConfig()
            .Map(dest => dest.Type, src => nameof(Truck))
            .Map(dest => dest.LoadCapacity, src => src.LoadCapacity);

        TypeAdapterConfig<BaseVehicle, VehicleDto>.NewConfig()
            .Include<Sedan, VehicleDto>()
            .Include<Hatchback, VehicleDto>()
            .Include<SUV, VehicleDto>()
            .Include<Truck, VehicleDto>();
    }
}

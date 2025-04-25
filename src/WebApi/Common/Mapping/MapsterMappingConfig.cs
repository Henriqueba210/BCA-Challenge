using Auction.Application.Features.Auction.Common;
using Auction.Application.Features.Vehicle.Common;
using Auction.Domain.Entities;
using Auction.Infrastructure.Models;

using Mapster;

using InfrastructureAuctionEntity = Auction.Infrastructure.Models.AuctionEntity;
using DomainAuctionEntity = Auction.Domain.Entities.AuctionEntity;


namespace Auction.Api.Common.Mapping;

public class MappingRegistration : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<DomainAuctionEntity, InfrastructureAuctionEntity>()
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.Bids, src => src.Bids);
        
        config.NewConfig<InfrastructureAuctionEntity, AuctionDto>()
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.Bids, src => src.Bids);

        config.NewConfig<VehicleEntity, BaseVehicle>()
            .Include<SedanEntity, Sedan>()
            .Include<HatchbackEntity, Hatchback>()
            .Include<SuvEntity, Suv>()
            .Include<TruckEntity, Truck>();

        config.NewConfig<SedanEntity, Sedan>()
            .Map(dest => dest.NumberOfDoors, src => src.NumberOfDoors);

        config.NewConfig<HatchbackEntity, Hatchback>()
            .Map(dest => dest.NumberOfDoors, src => src.NumberOfDoors);

        config.NewConfig<SuvEntity, Suv>()
            .Map(dest => dest.NumberOfSeats, src => src.NumberOfSeats);

        config.NewConfig<TruckEntity, Truck>()
            .Map(dest => dest.LoadCapacity, src => src.LoadCapacity);

        config.NewConfig<Infrastructure.Models.AuctionEntity, Domain.Entities.AuctionEntity>()
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.Bids, src => src.Bids);

        config.NewConfig<BidEntity, Bid>();
        config.NewConfig<Bid, BidDto>();

        config.NewConfig<BaseVehicle, VehicleDto>()
            .Include<Sedan, VehicleDto>()
            .Include<Hatchback, VehicleDto>()
            .Include<Suv, VehicleDto>()
            .Include<Truck, VehicleDto>();

        config.NewConfig<Sedan, VehicleDto>()
            .Map(dest => dest.Type, src => nameof(Sedan))
            .Map(dest => dest.NumberOfDoors, src => src.NumberOfDoors);

        config.NewConfig<Hatchback, VehicleDto>()
            .Map(dest => dest.Type, src => nameof(Hatchback))
            .Map(dest => dest.NumberOfDoors, src => src.NumberOfDoors);

        config.NewConfig<Suv, VehicleDto>()
            .Map(dest => dest.Type, src => nameof(Suv))
            .Map(dest => dest.NumberOfSeats, src => src.NumberOfSeats);

        config.NewConfig<Truck, VehicleDto>()
            .Map(dest => dest.Type, src => nameof(Truck))
            .Map(dest => dest.LoadCapacity, src => src.LoadCapacity);

        config.NewConfig<BaseVehicle, VehicleEntity>();

        config.NewConfig<VehicleEntity, VehicleDto>()
            .Map(dest => dest.Type, src => src.Type.ToString());
    }
}
using Application.Features.Auction.Common;
using Application.Features.Vehicle.Common;

using Auction.Domain.Entities;

using Mapster;

namespace Auction.Application.Features.Auction.Common;

public static class AuctionMappingConfig
{
    public static void Register()
    {
        TypeAdapterConfig<Bid, BidDto>.NewConfig();
        TypeAdapterConfig<AuctionEntity, AuctionDto>.NewConfig()
            .Map(dest => dest.Vehicle, src => src.VehicleVin.Value) // We'll handle this in the handler for full vehicle info
            .Map(dest => dest.Status, src => src.Status.ToString())
            .Map(dest => dest.Bids, src => src.Bids.Adapt<List<BidDto>>());
    }
}

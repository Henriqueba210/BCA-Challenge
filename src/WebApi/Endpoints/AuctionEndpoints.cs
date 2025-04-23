using Microsoft.AspNetCore.Mvc;

namespace Auction.Api.Endpoints;

public static class AuctionEndpoints
{
    public static void MapLocationEndpoints(this WebApplication app)
    {
        RouteGroupBuilder? group = app.MapGroup("/api/auction");
        
        
    }
}
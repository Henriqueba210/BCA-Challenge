using Auction.Api.Common.Mapping;
using Auction.Api.Contracts;
using Auction.Application.Features.Auction.Commands.Command;
using Auction.Application.Features.Auction.Common;

using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Auction.Api.Endpoints;

public static class BidEndpoints
{
    public static void MapBidEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/bid");

        group.MapPost("/", async ([FromBody] PlaceBidCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            if (!result.IsSuccess || result.Value is null)
            {
                return result.ToTypedResult<AuctionDto, AuctionResponseDto>();
            }

            var auctionDto = result.Value.Adapt<AuctionResponseDto>();
            return Results.Created($"/api/auction?vin={command.Vin}", auctionDto);
        })
        .WithName("Place Bid")
        .WithSummary("Place a bid on an active auction.")
        .WithDescription("Places a bid on an active auction for a vehicle. The bid amount must be greater than the current highest bid.")
        .Accepts<PlaceBidCommand>("application/json")
        .Produces<AuctionResponseDto>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}

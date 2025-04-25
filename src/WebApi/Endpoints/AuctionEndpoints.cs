using Auction.Application.Features.Auction.Common;
using Auction.Api.Common.Mapping;
using Auction.Api.Contracts;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

using Auction.Application.Features.Auction.Commands;
using Auction.Application.Features.Auction.Commands.Command;
using Auction.Application.Features.Auction.Queries.Query;

namespace Auction.Api.Endpoints;

public static class AuctionEndpoints
{
    public static void MapAuctionEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auction");

        group.MapPost("/start", async ([FromBody] StartAuctionCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            if (!result.IsSuccess || result.Value is null)
            {
                return result.ToTypedResult<AuctionDto, AuctionResponseDto>();
            }

            var auctionDto = result.Value.Adapt<AuctionResponseDto>();
            return Results.Created($"/api/auction?vin={command.Vin}", auctionDto);
        })
        // Explicitly specify the return type to Task<IResult> to fix async lambda issue
        .WithName("Start Auction")
        .WithSummary("Start an auction for a vehicle.")
        .WithDescription("Starts an auction for a vehicle by VIN. Only one auction can be active for a vehicle at a time.")
        .Accepts<StartAuctionCommand>("application/json")
        .Produces<AuctionResponseDto>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/close", async ([FromBody] CloseAuctionCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            if (!result.IsSuccess || result.Value is null)
            {
                return result.ToTypedResult<AuctionDto, AuctionResponseDto>();
            }
            
            var auctionDto = result.Value.Adapt<AuctionResponseDto>();
            return Results.Ok(auctionDto);
        })
        .WithName("Close Auction")
        .WithSummary("Close an active auction.")
        .WithDescription("Closes an active auction for a vehicle by VIN.")
        .Accepts<CloseAuctionCommand>("application/json")
        .Produces<AuctionDto>()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/", async ([FromQuery] string? status, [FromQuery] string? vehicleType, [FromQuery] string? manufacturer, [FromQuery] string? model, [FromQuery] int? year, ISender sender) =>
        {
            var query = new ListAuctionsQuery(status, vehicleType, manufacturer, model, year);
            var result = await sender.Send(query);
            return result.ToTypedResult<List<AuctionDto>, List<AuctionResponseDto>>();
        })
        .WithName("List Auctions")
        .WithSummary("List auctions with optional filters.")
        .WithDescription("Returns all auctions, optionally filtered by status, vehicle type, manufacturer, model, or year.")
        .Produces<List<AuctionResponseDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}

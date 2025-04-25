using Auction.Api.Common.Mapping;
using Auction.Application.Features.Vehicle.Common;
using Auction.Api.Contracts;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Auction.Application.Features.Vehicle.Commands;
using Auction.Application.Features.Vehicle.Commands.Command;
using Auction.Application.Features.Vehicle.Queries;
using Auction.Application.Features.Vehicle.Queries.Query;

namespace Auction.Api.Endpoints;

public static class VehicleEndpoints
{
    public static void MapVehicleEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auction/vehicle");

        group.MapPost("/", async ([FromBody] VehicleRequestDto dto, ISender sender) =>
        {
            var command = new AddVehicleCommand(
                dto.Vin,
                dto.Type,
                dto.Manufacturer,
                dto.Model,
                dto.Year,
                dto.StartingBid,
                dto.NumberOfDoors,
                dto.NumberOfSeats,
                dto.LoadCapacity
            );
            var result = await sender.Send(command);
            if (result.IsSuccess)
            {
                var vehicle = dto.Adapt<VehicleResponseDto>();
                return Results.Created($"/api/vehicle/{dto.Vin}", vehicle);
            }
            return result.ToTypedResult<VehicleDto, VehicleResponseDto>();
        })
        .WithName("Add Vehicle")
        .WithSummary("Add a new vehicle to the auction inventory.")
        .WithDescription("Adds a vehicle of a specific type (Sedan, SUV, Hatchback, Truck) to the auction inventory. VIN must be unique.")
        .Accepts<VehicleRequestDto>("application/json")
        .Produces<VehicleResponseDto>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/", async ([FromQuery] string? type, [FromQuery] string? manufacturer, [FromQuery] string? model, [FromQuery] int? year, ISender sender) =>
        {
            var query = new SearchVehiclesQuery(type, manufacturer, model, year);
            var result = await sender.Send(query);
            var dtos = result.Value.Select(v => v.Adapt<VehicleResponseDto>()).ToList();
            return Results.Ok(dtos);
        })
        .WithName("Search Vehicles")
        .WithSummary("Search vehicles in the auction inventory.")
        .WithDescription("Returns all vehicles matching the provided type, manufacturer, model, or year.")
        .Produces<List<VehicleResponseDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}

using Ardalis.Result;

using Auction.Application.Features.Auction.Common;

using MediatR;

namespace Auction.Application.Features.Auction.Queries.Query;

public record ListAuctionsQuery(
    string? Status = null,
    string? VehicleType = null,
    string? Manufacturer = null,
    string? Model = null,
    int? Year = null
) : IRequest<Result<List<AuctionDto>>>;

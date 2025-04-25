using Ardalis.Result;
using MediatR;
using Auction.Application.Features.Auction.Common;

namespace Application.Features.Auction.Queries;

public record ListAuctionsQuery(
    string? Status = null,
    string? VehicleType = null
) : IRequest<Result<List<AuctionDto>>>;

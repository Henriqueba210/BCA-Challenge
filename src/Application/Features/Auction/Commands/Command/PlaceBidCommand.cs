using Ardalis.Result;

using Auction.Application.Features.Auction.Common;

using MediatR;

namespace Auction.Application.Features.Auction.Commands.Command;

public record PlaceBidCommand(
    string Vin,
    decimal Amount,
    string Bidder
) : IRequest<Result<AuctionDto>>;

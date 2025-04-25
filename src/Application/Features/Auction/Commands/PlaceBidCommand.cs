using Ardalis.Result;
using MediatR;

namespace Application.Features.Auction.Commands;

public record PlaceBidCommand(
    string Vin,
    decimal Amount,
    string Bidder
) : IRequest<Result>;

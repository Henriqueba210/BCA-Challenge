using Ardalis.Result;

using MediatR;

namespace Auction.Application.Features.Auction.Commands;

public record CloseAuctionCommand(string Vin) : IRequest<Result>;

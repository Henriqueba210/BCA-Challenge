using Ardalis.Result;

using MediatR;

namespace Auction.Application.Features.Auction.Commands;

public record StartAuctionCommand(string Vin) : IRequest<Result>;

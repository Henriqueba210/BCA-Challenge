using Ardalis.Result;

using Auction.Application.Features.Auction.Common;

using MediatR;

namespace Auction.Application.Features.Auction.Commands.Command;

public record StartAuctionCommand(string Vin) : IRequest<Result<AuctionDto>>;

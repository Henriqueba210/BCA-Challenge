using Ardalis.Result;
using MediatR;

namespace Application.Features.Auction.Commands;

public record StartAuctionCommand(string Vin) : IRequest<Result>;

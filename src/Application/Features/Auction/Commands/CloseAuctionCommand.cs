using Ardalis.Result;
using MediatR;

namespace Application.Features.Auction.Commands;

public record CloseAuctionCommand(string Vin) : IRequest<Result>;

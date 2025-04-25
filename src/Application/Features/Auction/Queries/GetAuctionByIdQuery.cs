using Ardalis.Result;
using MediatR;
using Domain.Entities;

namespace Application.Features.Auction.Queries;

public record GetAuctionByIdQuery(Guid AuctionId) : IRequest<Result<Domain.Entities.Auction>>;

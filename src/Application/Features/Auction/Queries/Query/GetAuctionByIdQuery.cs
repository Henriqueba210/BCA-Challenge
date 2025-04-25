using Ardalis.Result;

using Auction.Application.Features.Auction.Common;

using MediatR;

namespace Auction.Application.Features.Auction.Queries.Query;

public record GetAuctionByIdQuery(Guid AuctionId) : IRequest<Result<AuctionDto>>;

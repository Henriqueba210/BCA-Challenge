using Ardalis.Result;

using Auction.Application.Features.Auction.Common;

using MediatR;

namespace Auction.Application.Features.Auction.Queries.Query;

public record GetAuctionByVinQuery(string Vin) : IRequest<Result<AuctionDto>>;

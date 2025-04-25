using Ardalis.Result;

using Auction.Application.Features.Auction.Common;
using Auction.Domain.Entities;

using MediatR;

namespace Application.Features.Auction.Queries;

public record GetAuctionByVinQuery(string Vin) : IRequest<Result<AuctionDto>>;

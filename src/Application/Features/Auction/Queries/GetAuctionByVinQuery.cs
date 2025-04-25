using Ardalis.Result;
using MediatR;
using Domain.Entities;

namespace Application.Features.Auction.Queries;

public record GetAuctionByVinQuery(string Vin) : IRequest<Result<Domain.Entities.Auction>>;

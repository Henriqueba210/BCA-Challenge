using Ardalis.Result;
using MediatR;
using Domain.Entities;
using System.Collections.Generic;

namespace Application.Features.Auction.Queries;

public record ListAuctionsQuery(
    string? Status = null,
    string? VehicleType = null
) : IRequest<Result<List<Domain.Entities.Auction>>>;

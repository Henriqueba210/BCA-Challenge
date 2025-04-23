using Ardalis.Result;

using MediatR;

namespace Auction.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
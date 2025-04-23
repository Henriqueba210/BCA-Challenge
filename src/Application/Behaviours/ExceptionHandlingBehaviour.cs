using Ardalis.Result;

using MediatR;

using Microsoft.Extensions.Logging;

using Serilog.Context;

namespace Auction.Application.Behaviours;

internal sealed class ExceptionHandlingBehavior<TRequest, TResponse>(
    ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            string requestName = typeof(TRequest).Name;

            using (LogContext.PushProperty("RequestName", requestName))
            {
                logger.LogError(ex, "Unhandled Exception occurred for Request {RequestName}", requestName);
            }

            Result? result = Result.CriticalError($"An error occurred processing request {requestName}");
            return (TResponse)result;
        }
    }
}
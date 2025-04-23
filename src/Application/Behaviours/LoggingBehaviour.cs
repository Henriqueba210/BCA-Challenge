using Ardalis.Result;

using MediatR;

using Microsoft.Extensions.Logging;

using Serilog.Context;

namespace Auction.Application.Behaviours;

internal sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;

        using (LogContext.PushProperty("RequestName", requestName))
        {
            logger.LogInformation("Processing request {RequestName}", requestName);

            TResponse result = await next();

            if (result.IsSuccess)
            {
                logger.LogInformation("Completed Request {RequestName}", requestName);
            }
            else
            {
                using (LogContext.PushProperty("Error", result.Errors, true))
                {
                    logger.LogError("Completed Request {RequestName} with error", requestName);
                }
            }

            return result;
        }
    }
}
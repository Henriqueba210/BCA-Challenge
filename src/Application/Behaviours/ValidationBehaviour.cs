using System.Reflection;

using Ardalis.Result;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

using Microsoft.Extensions.Logging;

using Serilog.Context;

namespace Auction.Application.Behaviours;

internal sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators,
    ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
    where TResponse : class, IResult
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next();
        }

        string requestName = typeof(TRequest).Name;
        ValidationContext<TRequest> validationContext = new(request);
        ValidationResult[] validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(validationContext, cancellationToken)));

        List<ValidationFailure> failures = validationResults
            .Where(r => r.Errors.Count != 0)
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Count == 0)
        {
            return await next();
        }

        using (LogContext.PushProperty("RequestName", requestName))
        using (LogContext.PushProperty("ValidationErrors", failures.Select(f => new { f.PropertyName, f.ErrorMessage }),
                   true))
        {
            logger.LogError("Validation failed for Request {RequestName}", requestName);
        }

        List<ValidationError> validationErrors = failures.Select(f => new ValidationError(f.ErrorMessage)).ToList();

        object? result = typeof(TResponse) switch
        {
            var t when t == typeof(Result) =>
                Result.Invalid(validationErrors),
            var t when t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Result<>) =>
                typeof(Result<>)
                    .MakeGenericType(t.GetGenericArguments())
                    .GetMethod(nameof(Result<object>.Invalid), new[] { typeof(List<ValidationError>) })!
                    .Invoke(null, [validationErrors]),
            _ => throw new InvalidOperationException("TResponse must be Result or Result<T>.")
        };

        return (TResponse)result!;
    }
}
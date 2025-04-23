using Ardalis.Result;

using Mapster;

using Microsoft.AspNetCore.Mvc;

using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Auction.Api.Common.Mapping;

public static class MapToTypedResult
{
    public static IResult ToTypedResult<T, TDestination>(this Result<T> result)
        where TDestination : class
    {
        return result.Status switch
        {
            ResultStatus.Ok => TypedResults.Ok(result.Value.Adapt<TDestination>()),
            ResultStatus.NotFound => TypedResults.NotFound(
                new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Resource Not Found",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Detail = result.Errors.FirstOrDefault() ?? "The requested resource was not found"
                }),
            ResultStatus.Invalid => TypedResults.BadRequest(
                new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation Error",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Detail = string.Join(", ", result.ValidationErrors.Select(e => e.ErrorMessage))
                }),
            _ => TypedResults.Problem(
                title: "Internal Server Error",
                detail: string.Join(", ", result.Errors),
                statusCode: StatusCodes.Status500InternalServerError,
                type: "https://tools.ietf.org/html/rfc7231#section-6.5.1")
        };
    }
}
using Microsoft.AspNetCore.Http;
using VSlices.Core.Abstracts.Responses;

// ReSharper disable once CheckNamespace
namespace OneOf;

public static class OneOfExtensions
{
    public static IResult MatchEndpointResult<TSuccess>(this OneOf<TSuccess, BusinessFailure> result,
        Func<TSuccess, IResult> successFunc)
    {
        return result.Match(
            successFunc,
            e =>
            {
                return e.Kind switch
                {
                    FailureKind.UserNotAllowed => TypedResults.Forbid(),
                    FailureKind.NotFoundResource => TypedResults.NotFound(),
                    FailureKind.ConcurrencyError => TypedResults.Conflict(),
                    FailureKind.Validation => TypedResults.UnprocessableEntity(e.Errors),
                    _ => throw new ArgumentOutOfRangeException(nameof(e.Kind), "A not valid FailureKind value was returned")
                };
            });
    }
}

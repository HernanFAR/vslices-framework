using Microsoft.AspNetCore.Http;

// ReSharper disable once CheckNamespace
namespace VSlices.Core.Abstracts.Responses;

public static class ResponseExtensions
{
    /// <summary>
    /// Maps a <see cref="Response{TSuccess}"/> to a <see cref="IResult"/>, using the provided function in success case.
    /// </summary>
    /// <typeparam name="TSuccess"></typeparam>
    /// <param name="result"></param>
    /// <param name="successFunc"></param>
    /// <returns>The <see cref="IResult"/> of the use case</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static IResult MatchEndpointResult<TSuccess>(this Response<TSuccess> result,
        Func<TSuccess, IResult> successFunc)
    {
        if (result.IsSuccess)
            return successFunc(result.SuccessValue);

        var businessFailure = result.BusinessFailure;

        return businessFailure.Kind switch
        {
            FailureKind.NotAllowedUser => TypedResults.Forbid(),
            FailureKind.NotFoundResource => businessFailure.Errors.Any()
                ? TypedResults.NotFound(businessFailure.Errors)
                : TypedResults.NotFound(),
            FailureKind.ConcurrencyError => businessFailure.Errors.Any()
                ? TypedResults.Conflict(businessFailure.Errors)
                : TypedResults.Conflict(),
            FailureKind.ContractValidation => TypedResults.UnprocessableEntity(businessFailure.Errors),
            FailureKind.DomainValidation => TypedResults.UnprocessableEntity(businessFailure.Errors),
            FailureKind.DefaultError => businessFailure.Errors.Any()
                ? TypedResults.BadRequest(businessFailure.Errors)
                : TypedResults.BadRequest(),
            FailureKind.NotAuthenticatedUser => TypedResults.Unauthorized(),
            FailureKind.UnhandledException => TypedResults.StatusCode(StatusCodes.Status500InternalServerError),
            _ => throw new ArgumentOutOfRangeException(nameof(businessFailure.Kind), "A not valid FailureKind value was returned")
        };
    }
}

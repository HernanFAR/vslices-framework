using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable once CheckNamespace
namespace VSlices.Core.Abstracts.Responses;

public static class ResponseExtensions
{
    /// <summary>
    /// Maps a <see cref="Response{TSuccess}"/> to a <see cref="IResult"/>, using the provided function in success case.
    /// <para>It uses the <see href="https://datatracker.ietf.org/doc/html/rfc9457"/></para>
    /// </summary>
    /// <typeparam name="TSuccess">Return type in success</typeparam>
    /// <param name="result">Result</param>
    /// <param name="successFunc">Function to execute in </param>
    /// <returns>The <see cref="IResult"/> of the use case</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static IResult MatchEndpointResult<TSuccess>(this Response<TSuccess> result,
        Func<TSuccess, IResult> successFunc)
    {
        if (result.IsSuccess)
            return successFunc(result.SuccessValue);

        var bf = result.BusinessFailure;

        return bf.Kind switch
        {
            FailureKind.Unspecified => bf.Errors.Any()
                ? TypedResults.Problem(new HttpValidationProblemDetails(bf.Errors.ToDictionary())
                {
                    Status = StatusCodes.Status400BadRequest,
                    Detail = bf.Detail,
                    Title = bf.Title ?? "The request could not be processed because of a client error."
                })
                : TypedResults.Problem(new HttpValidationProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Detail = bf.Detail,
                    Title = bf.Title ?? "The request could not be processed because of a client error."
                }),
            FailureKind.NotAuthenticatedUser => bf.Errors.Any()
                ? TypedResults.Problem(new HttpValidationProblemDetails(bf.Errors.ToDictionary())
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Detail = bf.Detail,
                    Title = bf.Title ?? "The request could not be processed because of a authentication error."
                })
                : TypedResults.Problem(new HttpValidationProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Detail = bf.Detail,
                    Title = bf.Title ?? "The request could not be processed because of a authentication error."
                }),
            FailureKind.NotAllowedUser => bf.Errors.Any()
                ? TypedResults.Problem(new HttpValidationProblemDetails(bf.Errors.ToDictionary())
                {
                    Status = StatusCodes.Status403Forbidden,
                    Detail = bf.Detail,
                    Title = bf.Title ?? "The request could not be processed because of a authentication error."
                })
                : TypedResults.Problem(new HttpValidationProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Detail = bf.Detail,
                    Title = bf.Title ?? "The request could not be processed because of a authentication error."
                }),
            FailureKind.NotFoundResource => bf.Errors.Any()
                ? TypedResults.Problem(new HttpValidationProblemDetails(bf.Errors.ToDictionary())
                {
                    Status = StatusCodes.Status404NotFound,
                    Detail = bf.Detail,
                    Title = bf.Title ?? "The requested resource was not found."
                })
                : TypedResults.Problem(new HttpValidationProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Detail = bf.Detail,
                    Title = bf.Title ?? "The requested resource was not found."
                }),
            FailureKind.ConcurrencyError => bf.Errors.Any()
                ? TypedResults.Problem(new HttpValidationProblemDetails(bf.Errors.ToDictionary())
                {
                    Status = StatusCodes.Status409Conflict,
                    Detail = bf.Detail,
                    Title = bf.Title ?? "The request could not be processed because of a concurrency error."
                })
                : TypedResults.Problem(new HttpValidationProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Detail = bf.Detail,
                    Title = bf.Title ?? "The request could not be processed because of a concurrency error."
                }),
            FailureKind.ContractValidation => TypedResults.Problem(new HttpValidationProblemDetails(bf.Errors.ToDictionary())
                {
                    Status = StatusCodes.Status422UnprocessableEntity,
                    Detail = bf.Detail,
                    Title = bf.Title ?? "The request could not be processed because of one or more validation errors."
                }),
            FailureKind.DomainValidation => TypedResults.Problem(new HttpValidationProblemDetails(bf.Errors.ToDictionary())
                {
                    Status = StatusCodes.Status422UnprocessableEntity,
                    Detail = bf.Detail,
                    Title = bf.Title ?? "The request could not be processed because of one or more validation errors."
                }),
            FailureKind.UnhandledException => bf.Errors.Any()
                ? TypedResults.Problem(new HttpValidationProblemDetails(bf.Errors.ToDictionary())
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = bf.Detail,
                    Title = bf.Title ?? "The request could not be processed because of a server error."
                })
                : TypedResults.Problem(new HttpValidationProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = bf.Detail,
                    Title = bf.Title ?? "The request could not be processed because of a server error."
                }),
            _ => throw new ArgumentOutOfRangeException(nameof(bf.Kind), "A not valid FailureKind value was returned")
        };
    }
}

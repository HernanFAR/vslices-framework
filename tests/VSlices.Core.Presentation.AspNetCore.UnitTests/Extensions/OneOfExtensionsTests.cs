using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Diagnostics;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.Presentation.AspNetCore.UnitTests.Extensions;

public class ResponseExtensionsTests
{
    [Fact]
    public void MatchEndpointResult_ShouldCallSuccessFunction()
    {
        Response<Success> oneOf = Success.Value;

        var result = oneOf.MatchEndpointResult(_ => TypedResults.Ok());

        result.Should().BeOfType<Ok>();
    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnProblemHttpResult_BadRequestStatusCode()
    {
        const string expTitle = "Title";
        const string expDetail = "Detail";

        Response<Success> oneOf = BusinessFailure.Of.Unspecified(expTitle, expDetail);

        var result = oneOf.MatchEndpointResult(_ => throw new UnreachableException());

        var problemDetails = result.Should().BeOfType<ProblemHttpResult>()
            .Subject.ProblemDetails;

        problemDetails.Status.Should().Be(StatusCodes.Status400BadRequest);
        problemDetails.Title.Should().Be(expTitle);
        problemDetails.Detail.Should().Be(expDetail);

    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnUnauthorizedResult()
    {
        const string expTitle = "Title";
        const string expDetail = "Detail";

        Response<Success> oneOf = BusinessFailure.Of.UserNotAuthenticated(expTitle, expDetail);

        var result = oneOf.MatchEndpointResult(_ => throw new UnreachableException());

        var problemDetails = result.Should().BeOfType<ProblemHttpResult>()
            .Subject.ProblemDetails;

        problemDetails.Status.Should().Be(StatusCodes.Status401Unauthorized);
        problemDetails.Title.Should().Be(expTitle);
        problemDetails.Detail.Should().Be(expDetail);

    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnProblemHttpResult_ForbiddenStatusCode()
    {
        const string expTitle = "Title";
        const string expDetail = "Detail";

        Response<Success> oneOf = BusinessFailure.Of.UserNotAllowed(expTitle, expDetail);

        var result = oneOf.MatchEndpointResult(_ => throw new UnreachableException());

        var problemDetails = result.Should().BeOfType<ProblemHttpResult>()
            .Subject.ProblemDetails;

        problemDetails.Status.Should().Be(StatusCodes.Status403Forbidden);
        problemDetails.Title.Should().Be(expTitle);
        problemDetails.Detail.Should().Be(expDetail);
    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnProblemHttpResult_NotFoundStatusCode()
    {
        const string expTitle = "Title";
        const string expDetail = "Detail";

        Response<Success> oneOf = BusinessFailure.Of.NotFoundResource(expTitle, expDetail);

        var result = oneOf.MatchEndpointResult(_ => throw new UnreachableException());

        var problemDetails = result.Should().BeOfType<ProblemHttpResult>()
            .Subject.ProblemDetails;

        problemDetails.Status.Should().Be(StatusCodes.Status404NotFound);
        problemDetails.Title.Should().Be(expTitle);
        problemDetails.Detail.Should().Be(expDetail);
    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnConflict()
    {
        const string expTitle = "Title";
        const string expDetail = "Detail";

        Response<Success> oneOf = BusinessFailure.Of.ConcurrencyError(expTitle, expDetail);

        var result = oneOf.MatchEndpointResult(_ => throw new UnreachableException());

        var problemDetails = result.Should().BeOfType<ProblemHttpResult>()
            .Subject.ProblemDetails;

        problemDetails.Status.Should().Be(StatusCodes.Status409Conflict);
        problemDetails.Title.Should().Be(expTitle);
        problemDetails.Detail.Should().Be(expDetail);
    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnUnprocessableEntity_DetailWithErrors()
    {
        const string expTitle = "Title";
        const string expDetail = "Detail";
        const string expErrorName1 = "ErrorName1";
        const string expErrorName2 = "ErrorName2";
        const string expErrorDetail1_1 = "ErrorDetail1";
        const string expErrorDetail1_2 = "ErrorDetail2";
        const string expErrorDetail2_1 = "ErrorDetail3";
        var validationErrors = new ValidationError[]
        {
            new (expErrorName1, expErrorDetail1_1),
            new (expErrorName1, expErrorDetail1_2),
            new (expErrorName2, expErrorDetail2_1)
        };

        Response<Success> oneOf = BusinessFailure.Of.ContractValidation(expTitle, expDetail, validationErrors);

        var result = oneOf.MatchEndpointResult(_ => throw new UnreachableException());

        var problemDetails = result.Should().BeOfType<ProblemHttpResult>()
            .Subject.ProblemDetails;

        problemDetails.Status.Should().Be(StatusCodes.Status422UnprocessableEntity);
        problemDetails.Title.Should().Be(expTitle);
        problemDetails.Detail.Should().Be(expDetail);
        ((HttpValidationProblemDetails)problemDetails).Errors
            .Should().BeEquivalentTo(new Dictionary<string, string[]>
            {
                { expErrorName1, new [] { expErrorDetail1_1, expErrorDetail1_2 } },
                { expErrorName2, new [] { expErrorDetail2_1 } }
            });

    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnUnprocessableEntity_DetailWithError()
    {
        const string expTitle = "Title";
        const string expDetail = "Detail";
        const string expErrorName1 = "ErrorName1";
        const string expErrorDetail1_1 = "ErrorDetail1";
        const string expErrorDetail1_2 = "ErrorDetail2";
        var validationErrors = new ValidationError[]
        {
            new (expErrorName1, expErrorDetail1_1),
            new (expErrorName1, expErrorDetail1_2),
        };

        Response<Success> oneOf = BusinessFailure.Of.ContractValidation(expTitle, expDetail, validationErrors);

        var result = oneOf.MatchEndpointResult(_ => throw new UnreachableException());

        var problemDetails = result.Should().BeOfType<ProblemHttpResult>()
            .Subject.ProblemDetails;

        problemDetails.Status.Should().Be(StatusCodes.Status422UnprocessableEntity);
        problemDetails.Title.Should().Be(expTitle);
        problemDetails.Detail.Should().Be(expDetail);
        ((HttpValidationProblemDetails)problemDetails).Errors
            .Should().BeEquivalentTo(new Dictionary<string, string[]>
            {
                { expErrorName1, new [] { expErrorDetail1_1, expErrorDetail1_2 } }
            });

    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnUnprocessableEntity_DetailWithErrorsAndDomainValidation()
    {
        const string expTitle = "Title";
        const string expDetail = "Detail";
        const string expErrorName1 = "ErrorName1";
        const string expErrorName2 = "ErrorName2";
        const string expErrorDetail1_1 = "ErrorDetail1";
        const string expErrorDetail1_2 = "ErrorDetail2";
        const string expErrorDetail2_1 = "ErrorDetail3";
        var validationErrors = new ValidationError[]
        {
            new (expErrorName1, expErrorDetail1_1),
            new (expErrorName1, expErrorDetail1_2),
            new (expErrorName2, expErrorDetail2_1)
        };

        Response<Success> oneOf = BusinessFailure.Of.DomainValidation(expTitle, expDetail, validationErrors);

        var result = oneOf.MatchEndpointResult(_ => throw new UnreachableException());

        var problemDetails = result.Should().BeOfType<ProblemHttpResult>()
            .Subject.ProblemDetails;

        problemDetails.Status.Should().Be(StatusCodes.Status422UnprocessableEntity);
        problemDetails.Title.Should().Be(expTitle);
        problemDetails.Detail.Should().Be(expDetail);
        ((HttpValidationProblemDetails)problemDetails).Errors
            .Should().BeEquivalentTo(new Dictionary<string, string[]>
            {
                { expErrorName1, new [] { expErrorDetail1_1, expErrorDetail1_2 } },
                { expErrorName2, new [] { expErrorDetail2_1 } }
            });

    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnUnprocessableEntity_DetailWithErrorAndDomainValidation()
    {
        const string expTitle = "Title";
        const string expDetail = "Detail";
        const string expErrorName1 = "ErrorName1";
        const string expErrorDetail1_1 = "ErrorDetail1";
        const string expErrorDetail1_2 = "ErrorDetail2";
        var validationErrors = new ValidationError[]
        {
            new (expErrorName1, expErrorDetail1_1),
            new (expErrorName1, expErrorDetail1_2),
        };

        Response<Success> oneOf = BusinessFailure.Of.DomainValidation(expTitle, expDetail, validationErrors);

        var result = oneOf.MatchEndpointResult(_ => throw new UnreachableException());

        var problemDetails = result.Should().BeOfType<ProblemHttpResult>()
            .Subject.ProblemDetails;

        problemDetails.Status.Should().Be(StatusCodes.Status422UnprocessableEntity);
        problemDetails.Title.Should().Be(expTitle);
        problemDetails.Detail.Should().Be(expDetail);
        ((HttpValidationProblemDetails)problemDetails).Errors
            .Should().BeEquivalentTo(new Dictionary<string, string[]>
            {
                { expErrorName1, new [] { expErrorDetail1_1, expErrorDetail1_2 } }
            });

    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnStatusCodeHttpResult()
    {
        const string expTitle = "Title";
        const string expDetail = "Detail";

        Response<Success> oneOf = BusinessFailure.Of.UnhandledException(expTitle, expDetail);

        var result = oneOf.MatchEndpointResult(_ => throw new UnreachableException());

        var problemDetails = result.Should().BeOfType<ProblemHttpResult>()
            .Subject.ProblemDetails;

        problemDetails.Status.Should().Be(StatusCodes.Status500InternalServerError);
        problemDetails.Title.Should().Be(expTitle);
        problemDetails.Detail.Should().Be(expDetail);

    }

    [Fact]
    public void MatchEndpointResult_ShouldThrowArgumentOutOfRange()
    {
        Response<Success> oneOf = new BusinessFailure((FailureKind)99, null, null, Array.Empty<ValidationError>());

        var act = () => oneOf.MatchEndpointResult(_ => throw new UnreachableException());

        act.Should().ThrowExactly<ArgumentOutOfRangeException>();

    }
}
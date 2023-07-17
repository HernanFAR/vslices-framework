using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VSlices.Core.Abstracts.Responses;
using NotFound = Microsoft.AspNetCore.Http.HttpResults.NotFound;

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
    public void MatchEndpointResult_ShouldCallReturnForbidHttpResult()
    {
        Response<Success> oneOf = BusinessFailure.Of.UserNotAllowed();

        var result = oneOf.MatchEndpointResult(_ => TypedResults.Ok());

        result.Should().BeOfType<ForbidHttpResult>();
    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnNotFound()
    {
        Response<Success> oneOf = BusinessFailure.Of.NotFoundResource();

        var result = oneOf.MatchEndpointResult(_ => TypedResults.Ok());

        result.Should().BeOfType<NotFound>();
    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnNotFound_DetailWithErrors()
    {
        Response<Success> oneOf = BusinessFailure.Of.NotFoundResource(new[] { "XD" });

        var result = oneOf.MatchEndpointResult(_ => TypedResults.Ok());

        result.Should().BeOfType<NotFound<string[]>>();

    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnConflict()
    {
        Response<Success> oneOf = BusinessFailure.Of.ConcurrencyError();

        var result = oneOf.MatchEndpointResult(_ => TypedResults.Ok());

        result.Should().BeOfType<Conflict>();
    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnConflict_DetailWithErrors()
    {
        Response<Success> oneOf = BusinessFailure.Of.ConcurrencyError(new[] { "XD" });

        var result = oneOf.MatchEndpointResult(_ => TypedResults.Ok());

        result.Should().BeOfType<Conflict<string[]>>();

    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnUnprocessableEntity_DetailWithErrors()
    {
        Response<Success> oneOf = BusinessFailure.Of.ContractValidation(new[] { "XD" });

        var result = oneOf.MatchEndpointResult(_ => TypedResults.Ok());

        result.Should().BeOfType<UnprocessableEntity<string[]>>();

    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnUnprocessableEntity_DetailWithError()
    {
        Response<Success> oneOf = BusinessFailure.Of.ContractValidation("XD");

        var result = oneOf.MatchEndpointResult(_ => TypedResults.Ok());

        result.Should().BeOfType<UnprocessableEntity<string[]>>();

    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnUnprocessableEntity_DetailWithErrorsAndDomainValidation()
    {
        Response<Success> oneOf = BusinessFailure.Of.DomainValidation(new[] { "XD" });

        var result = oneOf.MatchEndpointResult(_ => TypedResults.Ok());

        result.Should().BeOfType<UnprocessableEntity<string[]>>();

    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnUnprocessableEntity_DetailWithErrorAndDomainValidation()
    {
        Response<Success> oneOf = BusinessFailure.Of.DomainValidation("XD");

        var result = oneOf.MatchEndpointResult(_ => TypedResults.Ok());

        result.Should().BeOfType<UnprocessableEntity<string[]>>();

    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnBadRequest_DetailWithErrors()
    {
        Response<Success> oneOf = BusinessFailure.Of.DefaultError(new [] {"XD" });

        var result = oneOf.MatchEndpointResult(_ => TypedResults.Ok());

        result.Should().BeOfType<BadRequest<string[]>>();

    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnBadRequest_DetailWithError()
    {
        Response<Success> oneOf = BusinessFailure.Of.DefaultError("XD");

        var result = oneOf.MatchEndpointResult(_ => TypedResults.Ok());

        result.Should().BeOfType<BadRequest<string[]>>();

    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnBadRequest()
    {
        Response<Success> oneOf = BusinessFailure.Of.DefaultError();

        var result = oneOf.MatchEndpointResult(_ => TypedResults.Ok());

        result.Should().BeOfType<BadRequest>();

    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnUnauthorizedResult()
    {
        Response<Success> oneOf = BusinessFailure.Of.UserNotAuthenticated();

        var result = oneOf.MatchEndpointResult(_ => TypedResults.Ok());

        result.Should().BeOfType<UnauthorizedHttpResult>();

    }

    [Fact]
    public void MatchEndpointResult_ShouldCallReturnStatusCodeHttpResult()
    {
        Response<Success> oneOf = BusinessFailure.Of.UnhandledException();

        var result = oneOf.MatchEndpointResult(_ => TypedResults.Ok());

        result.Should().BeOfType<StatusCodeHttpResult>();

    }

    [Fact]
    public void MatchEndpointResult_ShouldThrowArgumentOutOfRange()
    {
        Response<Success> oneOf = new BusinessFailure((FailureKind)99, Array.Empty<string>());

        var act = () => oneOf.MatchEndpointResult(_ => TypedResults.Ok());

        act.Should().ThrowExactly<ArgumentOutOfRangeException>();

    }
}
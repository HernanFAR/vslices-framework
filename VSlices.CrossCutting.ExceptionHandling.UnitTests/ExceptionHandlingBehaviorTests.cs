using FluentAssertions;
using Moq;
using VSlices.Core.Abstracts.Handlers;
using VSlices.Core.Abstracts.Requests;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.CrossCutting.ExceptionHandling.UnitTests;

public class ExceptionHandlingBehaviorTests
{
    public record Request : IRequest;

    [Fact]
    public async Task HandleAsync_ShouldReturnResponse()
    {
        var request = new Request();

        var pipelineMock = new Mock<ExceptionHandlingBehavior<Request, Success>>
        {
            CallBase = true
        };
        var pipeline = pipelineMock.Object;

        RequestHandlerDelegate<Success> handler = () => ValueTask.FromResult<Response<Success>>(Success.Value);

        var result = await pipeline.HandleAsync(request, handler);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnException()
    {
        var request = new Request();

        var pipelineMock = new Mock<ExceptionHandlingBehavior<Request, Success>>
        {
            CallBase = true
        };
        var pipeline = pipelineMock.Object;
        var ex = new Exception();

        pipelineMock.Setup(e => e.ProcessExceptionAsync(ex, request))
            .Verifiable();

        RequestHandlerDelegate<Success> handler = () => throw ex;

        var result = await pipeline.HandleAsync(request, handler);

        pipelineMock.Verify();

        result.IsFailure.Should().BeTrue();
        result.BusinessFailure.Kind.Should().Be(FailureKind.UnhandledException);
    }
}
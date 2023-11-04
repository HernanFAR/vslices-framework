using FluentAssertions;
using Moq;
using VSlices.Core.DataAccess.Abstracts;
using VSlices.Core.Abstracts.Responses;
using VSlices.Core.Abstracts.Requests;

namespace VSlices.Core.Handlers.UnitTests.ReadHandlers;

public class ReadHandler_TwoGenerics
{
    public record Request : IQuery<Response>;
    public record Response;

    private readonly Mock<IReadRepository<Response, Request>> _mockedRepository;
    private readonly Mock<ReadHandler<Request, Response>> _mockedHandler;

    public ReadHandler_TwoGenerics()
    {
        _mockedRepository = new Mock<IReadRepository<Response, Request>>();
        _mockedHandler = new Mock<ReadHandler<Request, Response>>(_mockedRepository.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnBusinessFailure_DetailCallValidateUseCaseRulesAsync()
    {
        var request = new Request();
        var businessFailure = BusinessFailure.Of.NotFoundResource();

        _mockedHandler.Setup(e => e.HandleAsync(request, default))
            .CallBase();
        _mockedHandler.Setup(e => e.ValidateFeatureRulesAsync(request, default))
            .ReturnsAsync(businessFailure);

        var handlerResponse = await _mockedHandler.Object.HandleAsync(request, default);

        handlerResponse.BusinessFailure.Should().Be(businessFailure);

        _mockedHandler.Verify(e => e.HandleAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateFeatureRulesAsync(request, default), Times.Once);
        _mockedHandler.VerifyNoOtherCalls();

        _mockedRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnBusinessFailure_DetailCallValidateUseCaseRulesAsyncAndRequestToSearchOptionsAsyncAndReadAsync()
    {
        var request = new Request();
        var businessFailure = BusinessFailure.Of.NotFoundResource();
        var success = Success.Value;

        _mockedHandler.Setup(e => e.HandleAsync(request, default))
            .CallBase();
        _mockedHandler.Setup(e => e.ValidateFeatureRulesAsync(request, default))
            .ReturnsAsync(success);

        _mockedRepository.Setup(e => e.ReadAsync(request, default))
            .ReturnsAsync(businessFailure);

        var handlerResponse = await _mockedHandler.Object.HandleAsync(request, default);

        handlerResponse.BusinessFailure.Should().Be(businessFailure);

        _mockedHandler.Verify(e => e.HandleAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateFeatureRulesAsync(request, default), Times.Once);
        _mockedHandler.VerifyNoOtherCalls();

        _mockedRepository.Verify(e => e.ReadAsync(request, default));
        _mockedRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnResponse()
    {
        var request = new Request();
        var response = new Response();
        var success = Success.Value;

        _mockedHandler.Setup(e => e.HandleAsync(request, default))
            .CallBase();
        _mockedHandler.Setup(e => e.ValidateFeatureRulesAsync(request, default))
            .ReturnsAsync(success);

        _mockedRepository.Setup(e => e.ReadAsync(request, default))
            .ReturnsAsync(response);

        var handlerResponse = await _mockedHandler.Object.HandleAsync(request, default);

        handlerResponse.SuccessValue.Should().Be(response);

        _mockedHandler.Verify(e => e.HandleAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateFeatureRulesAsync(request, default), Times.Once);
        _mockedHandler.VerifyNoOtherCalls();

        _mockedRepository.Verify(e => e.ReadAsync(request, default));
        _mockedRepository.VerifyNoOtherCalls();
    }
}

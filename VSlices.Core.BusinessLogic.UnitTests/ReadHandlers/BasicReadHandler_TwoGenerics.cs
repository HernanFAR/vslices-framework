﻿using FluentAssertions;
using Moq;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.DataAccess.Abstracts;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic.UnitTests.ReadHandlers;

public class BasicReadHandler_TwoGenerics
{
    public record Response;
    public record Request : IQuery<Response>;

    private readonly Mock<IReadRepository<Response>> _mockedRepository;
    private readonly Mock<BasicReadHandler<Request, Response>> _mockedHandler;

    public BasicReadHandler_TwoGenerics()
    {
        _mockedRepository = new Mock<IReadRepository<Response>>();
        _mockedHandler = new Mock<BasicReadHandler<Request, Response>>(_mockedRepository.Object);
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

        _mockedRepository.Setup(e => e.ReadAsync(default))
            .ReturnsAsync(businessFailure);

        var handlerResponse = await _mockedHandler.Object.HandleAsync(request, default);

        handlerResponse.BusinessFailure.Should().Be(businessFailure);

        _mockedHandler.Verify(e => e.HandleAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateFeatureRulesAsync(request, default), Times.Once);
        _mockedHandler.VerifyNoOtherCalls();

        _mockedRepository.Verify(e => e.ReadAsync(default));
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

        _mockedRepository.Setup(e => e.ReadAsync(default))
            .ReturnsAsync(response);

        var handlerResponse = await _mockedHandler.Object.HandleAsync(request, default);

        handlerResponse.SuccessValue.Should().Be(response);

        _mockedHandler.Verify(e => e.HandleAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateFeatureRulesAsync(request, default), Times.Once);
        _mockedHandler.VerifyNoOtherCalls();

        _mockedRepository.Verify(e => e.ReadAsync(default));
        _mockedRepository.VerifyNoOtherCalls();
    }
}
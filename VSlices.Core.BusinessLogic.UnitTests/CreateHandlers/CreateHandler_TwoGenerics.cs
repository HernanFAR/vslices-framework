using FluentAssertions;
using Moq;
using Moq.Protected;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.DataAccess.Abstracts;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic.UnitTests.CreateHandlers;

public class CreateHandler_TwoGenerics
{
    public record Domain;
    public record Request : ICommand;

    private readonly Mock<ICreateRepository<Domain>> _mockedRepository;
    private readonly Mock<CreateHandler<Request, Domain>> _mockedHandler;

    public CreateHandler_TwoGenerics()
    {
        _mockedRepository = new Mock<ICreateRepository<Domain>>();
        _mockedHandler = new Mock<CreateHandler<Request, Domain>>(_mockedRepository.Object);
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
    public async Task HandleAsync_ShouldReturnBusinessFailure_DetailCallValidateUseCaseRulesAsyncAndGetDomainEntityAsyncAndCreateAsync()
    {
        var request = new Request();
        var domain = new Domain();

        var success = Success.Value;
        var businessFailure = BusinessFailure.Of.NotFoundResource();

        _mockedHandler.Setup(e => e.HandleAsync(request, default))
            .CallBase();
        _mockedHandler.Setup(e => e.ValidateFeatureRulesAsync(request, default))
            .ReturnsAsync(success);
        _mockedHandler.Setup(e => e.CreateEntityAsync(request, default))
            .ReturnsAsync(domain);

        _mockedRepository.Setup(e => e.CreateAsync(domain, default))
            .ReturnsAsync(businessFailure);

        var handlerResponse = await _mockedHandler.Object.HandleAsync(request, default);

        handlerResponse.BusinessFailure.Should().Be(businessFailure);

        _mockedHandler.Verify(e => e.HandleAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateFeatureRulesAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.CreateEntityAsync(request, default), Times.Once);
        _mockedHandler.VerifyNoOtherCalls();

        _mockedRepository.Verify(e => e.CreateAsync(domain, default), Times.Once);
        _mockedRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess()
    {
        var request = new Request();
        var domain = new Domain();

        var success = Success.Value;

        _mockedHandler.Setup(e => e.HandleAsync(request, default))
            .CallBase();
        _mockedHandler.Setup(e => e.AfterCreationAsync(domain, request, default))
            .CallBase();
        _mockedHandler.Setup(e => e.ValidateFeatureRulesAsync(request, default))
            .ReturnsAsync(success);
        _mockedHandler.Setup(e => e.CreateEntityAsync(request, default))
            .ReturnsAsync(domain);
        _mockedHandler.Setup(e => e.GetResponseAsync(domain, request, default))
            .CallBase();

        _mockedRepository.Setup(e => e.CreateAsync(domain, default))
            .ReturnsAsync(domain);

        var handlerResponse = await _mockedHandler.Object.HandleAsync(request, default);

        handlerResponse.SuccessValue.Should().Be(success);

        _mockedHandler.Verify(e => e.HandleAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateFeatureRulesAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.CreateEntityAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.AfterCreationAsync(domain, request, default), Times.Once);
        _mockedHandler.Protected().Verify("GetResponseAsync", Times.Once(), domain, request, default(CancellationToken));
        _mockedHandler.VerifyNoOtherCalls();

        _mockedRepository.Verify(e => e.CreateAsync(domain, default), Times.Once);
        _mockedRepository.VerifyNoOtherCalls();
    }
}
using FluentAssertions;
using Moq;
using VSlices.Core.DataAccess.Abstracts;
using VSlices.Core.Abstracts.Responses;
using VSlices.Core.Abstracts.Requests;

namespace VSlices.Core.Handlers.UnitTests.CreateHandlers;

public class DomainValidatedCreateHandler_ThreeGenerics
{
    public record Domain;
    public record Response;
    public record Request : ICommand<Response>;

    private readonly Mock<ICreateRepository<Domain>> _mockedRepository;
    private readonly Mock<EntityValidatedCreateHandler<Request, Response, Domain>> _mockedHandler;

    public DomainValidatedCreateHandler_ThreeGenerics()
    {
        _mockedRepository = new Mock<ICreateRepository<Domain>>();
        _mockedHandler = new Mock<EntityValidatedCreateHandler<Request, Response, Domain>>(_mockedRepository.Object);
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
    public async Task HandleAsync_ShouldReturnBusinessFailure_DetailCallValidateUseCaseRulesAsyncAndGetDomainEntityAsyncAndValidateRequestAsync()
    {
        var request = new Request();
        var domain = new Domain();

        var businessFailure = BusinessFailure.Of.NotFoundResource();
        var success = Success.Value;

        _mockedHandler.Setup(e => e.HandleAsync(request, default))
            .CallBase();
        _mockedHandler.Setup(e => e.ValidateFeatureRulesAsync(request, default))
            .ReturnsAsync(success);
        _mockedHandler.Setup(e => e.CreateEntityAsync(request, default))
            .ReturnsAsync(domain);
        _mockedHandler.Setup(e => e.ValidateEntityAsync(domain, default))
            .ReturnsAsync(businessFailure);

        var handlerResponse = await _mockedHandler.Object.HandleAsync(request, default);

        handlerResponse.BusinessFailure.Should().Be(businessFailure);

        _mockedHandler.Verify(e => e.HandleAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateFeatureRulesAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.CreateEntityAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateEntityAsync(domain, default), Times.Once);
        _mockedHandler.VerifyNoOtherCalls();

        _mockedRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnBusinessFailure_DetailCallValidateUseCaseRulesAsyncAndGetDomainEntityAsyncAndValidateDomainAsyncAndCreateAsync()
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
        _mockedHandler.Setup(e => e.ValidateEntityAsync(domain, default))
            .ReturnsAsync(success);

        _mockedRepository.Setup(e => e.CreateAsync(domain, default))
            .ReturnsAsync(businessFailure);

        var handlerResponse = await _mockedHandler.Object.HandleAsync(request, default);

        handlerResponse.BusinessFailure.Should().Be(businessFailure);

        _mockedHandler.Verify(e => e.HandleAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateFeatureRulesAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.CreateEntityAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateEntityAsync(domain, default), Times.Once);
        _mockedHandler.VerifyNoOtherCalls();

        _mockedRepository.Verify(e => e.CreateAsync(domain, default), Times.Once);
        _mockedRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess()
    {
        var request = new Request();
        var domain = new Domain();
        var response = new Response();

        var success = Success.Value;

        _mockedHandler.Setup(e => e.HandleAsync(request, default))
            .CallBase();
        _mockedHandler.Setup(e => e.AfterCreationAsync(domain, request, default))
            .CallBase();
        _mockedHandler.Setup(e => e.ValidateFeatureRulesAsync(request, default))
            .ReturnsAsync(success);
        _mockedHandler.Setup(e => e.CreateEntityAsync(request, default))
            .ReturnsAsync(domain);
        _mockedHandler.Setup(e => e.ValidateEntityAsync(domain, default))
            .ReturnsAsync(success);
        _mockedHandler.Setup(e => e.GetResponseAsync(domain, request, default))
            .ReturnsAsync(response);

        _mockedRepository.Setup(e => e.CreateAsync(domain, default))
            .ReturnsAsync(domain);

        var handlerResponse = await _mockedHandler.Object.HandleAsync(request, default);

        handlerResponse.SuccessValue.Should().Be(response);

        _mockedHandler.Verify(e => e.HandleAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateFeatureRulesAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.CreateEntityAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateEntityAsync(domain, default), Times.Once);
        _mockedHandler.Verify(e => e.AfterCreationAsync(domain, request, default), Times.Once);
        _mockedHandler.Verify(e => e.GetResponseAsync(domain, request, default), Times.Once);
        _mockedHandler.VerifyNoOtherCalls();

        _mockedRepository.Verify(e => e.CreateAsync(domain, default), Times.Once);
        _mockedRepository.VerifyNoOtherCalls();
    }
}
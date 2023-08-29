using FluentAssertions;
using Moq;
using Moq.Protected;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic.UnitTests.UpdateHandlers;

public class UpdateHandler_TwoGenerics
{
    public record Domain;
    public record Request : ICommand;

    private readonly Mock<IUpdateRepository<Domain>> _mockedRepository;
    private readonly Mock<UpdateHandler<Request, Domain>> _mockedHandler;

    public UpdateHandler_TwoGenerics()
    {
        _mockedRepository = new Mock<IUpdateRepository<Domain>>();
        _mockedHandler = new Mock<UpdateHandler<Request, Domain>>(_mockedRepository.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnBusinessFailure_DetailCallValidateUseCaseRulesAsync()
    {
        var request = new Request();
        var businessFailure = BusinessFailure.Of.NotFoundResource();

        _mockedHandler.Setup(e => e.HandleAsync(request, default))
            .CallBase();
        _mockedHandler.Setup(e => e.ValidateUseCaseRulesAsync(request, default))
            .ReturnsAsync(businessFailure);

        var handlerResponse = await _mockedHandler.Object.HandleAsync(request, default);

        handlerResponse.BusinessFailure.Should().Be(businessFailure);

        _mockedHandler.Verify(e => e.HandleAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateUseCaseRulesAsync(request, default), Times.Once);
        _mockedHandler.VerifyNoOtherCalls();

        _mockedRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnBusinessFailure_DetailCallValidateUseCaseRulesAsyncAndGetDomainEntityAsyncAndUpdateAsync()
    {
        var request = new Request();
        var domain = new Domain();

        var success = Success.Value;
        var businessFailure = BusinessFailure.Of.NotFoundResource();

        _mockedHandler.Setup(e => e.HandleAsync(request, default))
            .CallBase();
        _mockedHandler.Setup(e => e.ValidateUseCaseRulesAsync(request, default))
            .ReturnsAsync(success);
        _mockedHandler.Setup(e => e.GetAndProcessEntityAsync(request, default))
            .ReturnsAsync(domain);

        _mockedRepository.Setup(e => e.UpdateAsync(domain, default))
            .ReturnsAsync(businessFailure);

        var handlerResponse = await _mockedHandler.Object.HandleAsync(request, default);

        handlerResponse.BusinessFailure.Should().Be(businessFailure);

        _mockedHandler.Verify(e => e.HandleAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateUseCaseRulesAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.GetAndProcessEntityAsync(request, default), Times.Once);
        _mockedHandler.VerifyNoOtherCalls();

        _mockedRepository.Verify(e => e.UpdateAsync(domain, default), Times.Once);
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
        _mockedHandler.Setup(e => e.ValidateUseCaseRulesAsync(request, default))
            .ReturnsAsync(success);
        _mockedHandler.Setup(e => e.GetAndProcessEntityAsync(request, default))
            .ReturnsAsync(domain);
        _mockedRepository.Setup(e => e.UpdateAsync(domain, default))
            .ReturnsAsync(domain);
        _mockedHandler.Setup(e => e.GetResponseAsync(domain, request, default))
            .CallBase();

        var handlerResponse = await _mockedHandler.Object.HandleAsync(request, default);

        handlerResponse.SuccessValue.Should().Be(success);

        _mockedHandler.Verify(e => e.HandleAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.ValidateUseCaseRulesAsync(request, default), Times.Once);
        _mockedHandler.Verify(e => e.GetAndProcessEntityAsync(request, default), Times.Once);
        _mockedHandler.Protected().Verify("GetResponseAsync", Times.Once(), domain, request, default(CancellationToken));
        _mockedHandler.VerifyNoOtherCalls();

        _mockedRepository.Verify(e => e.UpdateAsync(domain, default), Times.Once);
        _mockedRepository.VerifyNoOtherCalls();
    }
}
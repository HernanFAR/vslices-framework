using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Moq.Protected;
using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic.FluentValidation.UnitTests.RemoveHandlers;


public class RequestFluentValidatedRemoveHandler_ThreeGenerics
{
    public record Domain;
    public record Request;
    public record Response;

    public class RequestFluentValidatedRemoveHandler : RequestFluentValidatedRemoveHandler<Request, Response, Domain>
    {
        public RequestFluentValidatedRemoveHandler(IValidator<Request> requestValidator, IRemovableRepository<Domain> repository) : base(requestValidator, repository) { }

        protected override async ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(Request request, CancellationToken cancellationToken = default) 
            => new Success();

        protected override async ValueTask<Domain> GetDomainEntityAsync(Request request, CancellationToken cancellationToken = default)
            => new Domain();

        protected override async ValueTask<Response> GetResponseAsync(Domain domainEntity, Request request, CancellationToken cancellationToken = default)
            => new Response();

    }

    private readonly Mock<IValidator<Request>> _mockedValidator;
    private readonly Mock<IRemovableRepository<Domain>> _mockedRepository;
    private readonly RequestFluentValidatedRemoveHandler _handler;

    public RequestFluentValidatedRemoveHandler_ThreeGenerics()
    {
        _mockedValidator = new Mock<IValidator<Request>>();
        _mockedRepository = new Mock<IRemovableRepository<Domain>>();
        _handler = new RequestFluentValidatedRemoveHandler(_mockedValidator.Object, _mockedRepository.Object);
    }

    [Fact]
    public async Task ValidateAsync_ReturnBusinessFailure()
    {
        const string validationFailureString = "TestingTesting";
        
        var request = new Request();
        var validationResult = new ValidationResult(
            new List<ValidationFailure>
            {
                new (string.Empty, validationFailureString)
            });

        _mockedValidator.Setup(e => e.ValidateAsync(request, default))
            .ReturnsAsync(validationResult)
            .Verifiable();

        var handlerResponse = await _handler.HandleAsync(request);

        handlerResponse.IsT1.Should().BeTrue();
        handlerResponse.AsT1
            .Errors.Should().ContainSingle(e => e == validationFailureString);
        handlerResponse.AsT1
            .Kind.Should().Be(FailureKind.Validation);

        _mockedValidator.Verify();
        _mockedValidator.VerifyNoOtherCalls();

        _mockedRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ValidateAsync_ReturnResponse()
    {
        var request = new Request();

        _mockedValidator.Setup(e => e.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult())
            .Verifiable();

        _mockedRepository.Setup(e => e.RemoveAsync(It.IsAny<Domain>(), default))
            .ReturnsAsync(new Success())
            .Verifiable();

        var handlerResponse = await _handler.HandleAsync(request);

        handlerResponse.IsT0.Should().BeTrue();
        handlerResponse.AsT0
            .Should().BeOfType<Response>();

        _mockedValidator.Verify();
        _mockedValidator.VerifyNoOtherCalls();

        _mockedRepository.Verify();
        _mockedRepository.VerifyNoOtherCalls();
    }
}

using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Moq.Protected;
using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic.FluentValidation.UnitTests.UpdateHandlers;


public class RequestFluentValidatedRemoveHandler_TwoGenerics
{
    public record Domain;
    public record Request : ICommand;

    public class RequestFluentValidatedUpdateHandler : RequestFluentValidatedUpdateHandler<Request, Domain>
    {
        public RequestFluentValidatedUpdateHandler(IValidator<Request> requestValidator, IUpdateRepository<Domain> repository) : base(requestValidator, repository) { }

        protected override ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(Request request, CancellationToken cancellationToken = default) 
            => ValueTask.FromResult<OneOf<Success, BusinessFailure>>(new Success());

        protected override ValueTask<Domain> GetAndProcessEntityAsync(Request request, CancellationToken cancellationToken = default)
            => ValueTask.FromResult(new Domain());
        
    }

    private readonly Mock<IValidator<Request>> _mockedValidator;
    private readonly Mock<IUpdateRepository<Domain>> _mockedRepository;
    private readonly RequestFluentValidatedUpdateHandler _handler;

    public RequestFluentValidatedRemoveHandler_TwoGenerics()
    {
        _mockedValidator = new Mock<IValidator<Request>>();
        _mockedRepository = new Mock<IUpdateRepository<Domain>>();
        _handler = new RequestFluentValidatedUpdateHandler(_mockedValidator.Object, _mockedRepository.Object);
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
        var domain = new Domain();

        _mockedValidator.Setup(e => e.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult())
            .Verifiable();

        _mockedRepository.Setup(e => e.UpdateAsync(domain, default))
            .ReturnsAsync(domain)
            .Verifiable();

        var handlerResponse = await _handler.HandleAsync(request);

        handlerResponse.IsT0.Should().BeTrue();

        _mockedValidator.Verify();
        _mockedValidator.VerifyNoOtherCalls();

        _mockedRepository.Verify();
        _mockedRepository.VerifyNoOtherCalls();
    }
}

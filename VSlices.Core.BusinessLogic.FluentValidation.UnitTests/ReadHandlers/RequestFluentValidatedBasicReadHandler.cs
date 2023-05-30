using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic.FluentValidation.UnitTests.ReadHandlers;

public class RequestFluentValidatedBasicReadHandler
{
    public record Request : IQuery;
    public record Response;

    public class Handler : RequestFluentValidatedBasicReadHandler<Request, Response>
    {
        public Handler(IValidator<Request> requestValidator, IReadRepository<Response> repository) : base(requestValidator, repository) { }
        
        protected override ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(Request request, CancellationToken cancellationToken = default)
            => ValueTask.FromResult<OneOf<Success, BusinessFailure>>(new Success());

    }

    private readonly Mock<IValidator<Request>> _mockedValidator;
    private readonly Mock<IReadRepository<Response>> _mockedRepository;
    private readonly Handler _handler;

    public RequestFluentValidatedBasicReadHandler()
    {
        _mockedValidator = new Mock<IValidator<Request>>();
        _mockedRepository = new Mock<IReadRepository<Response>>();
        _handler = new Handler(_mockedValidator.Object, _mockedRepository.Object);
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
        var response = new Response();

        _mockedValidator.Setup(e => e.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult())
            .Verifiable();

        _mockedRepository.Setup(e => e.ReadAsync(default))
            .ReturnsAsync(response)
            .Verifiable();

        var handlerResponse = await _handler.HandleAsync(request);

        handlerResponse.IsT0.Should().BeTrue();
        handlerResponse.AsT0
            .Should().Be(response);

        _mockedValidator.Verify();
        _mockedValidator.VerifyNoOtherCalls();

        _mockedRepository.Verify();
        _mockedRepository.VerifyNoOtherCalls();
    }
}

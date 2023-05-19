﻿using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Moq.Protected;
using OneOf;
using OneOf.Types;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic.FluentValidation.UnitTests.CreateHandlers;


public class DomainFluentValidatedUpdateHandler_TwoGenerics
{
    public record Domain;
    public record Request;

    public class DomainFluentValidatedCreateHandler : DomainFluentValidatedCreateHandler<Request, Domain>
    {
        public DomainFluentValidatedCreateHandler(IValidator<Domain> requestValidator, ICreatableRepository<Domain> repository) : base(requestValidator, repository) { }

        protected override async ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(Request request, CancellationToken cancellationToken = default) 
            => new Success();

        protected override async ValueTask<Domain> GetDomainEntityAsync(Request request, CancellationToken cancellationToken = default)
            => new Domain();

    }

    private readonly Mock<IValidator<Domain>> _mockedValidator;
    private readonly Mock<ICreatableRepository<Domain>> _mockedRepository;
    private readonly DomainFluentValidatedCreateHandler _handler;

    public DomainFluentValidatedUpdateHandler_TwoGenerics()
    {
        _mockedValidator = new Mock<IValidator<Domain>>();
        _mockedRepository = new Mock<ICreatableRepository<Domain>>();
        _handler = new DomainFluentValidatedCreateHandler(_mockedValidator.Object, _mockedRepository.Object);
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

        _mockedValidator.Setup(e => e.ValidateAsync(It.IsAny<Domain>(), default))
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

        _mockedValidator.Setup(e => e.ValidateAsync(It.IsAny<Domain>(), default))
            .ReturnsAsync(new ValidationResult())
            .Verifiable();

        _mockedRepository.Setup(e => e.CreateAsync(It.IsAny<Domain>(), default))
            .ReturnsAsync(new Success())
            .Verifiable();

        var handlerResponse = await _handler.HandleAsync(request);

        handlerResponse.IsT0.Should().BeTrue();

        _mockedValidator.Verify();
        _mockedValidator.VerifyNoOtherCalls();

        _mockedRepository.Verify();
        _mockedRepository.VerifyNoOtherCalls();
    }
}

﻿using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.DataAccess.Abstracts;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.Core.BusinessLogic.FluentValidation.UnitTests.RemoveHandlers;


public class DomainFluentValidatedRemoveHandler_TwoGenerics
{
    public record Domain;
    public record Request : ICommand;

    public class EntityFluentValidatedRemoveHandler : EntityFluentValidatedRemoveHandler<Request, Domain>
    {
        public EntityFluentValidatedRemoveHandler(IValidator<Domain> requestValidator, IRemoveRepository<Domain> repository) : base(requestValidator, repository) { }

        protected override ValueTask<Response<Success>> ValidateFeatureRulesAsync(Request request, CancellationToken cancellationToken = default)
            => ValueTask.FromResult<Response<Success>>(Success.Value);

        protected override ValueTask<Domain> GetAndProcessEntityAsync(Request request, CancellationToken cancellationToken = default)
            => ValueTask.FromResult(new Domain());

    }

    private readonly Mock<IValidator<Domain>> _mockedValidator;
    private readonly Mock<IRemoveRepository<Domain>> _mockedRepository;
    private readonly EntityFluentValidatedRemoveHandler _handler;

    public DomainFluentValidatedRemoveHandler_TwoGenerics()
    {
        _mockedValidator = new Mock<IValidator<Domain>>();
        _mockedRepository = new Mock<IRemoveRepository<Domain>>();
        _handler = new EntityFluentValidatedRemoveHandler(_mockedValidator.Object, _mockedRepository.Object);
    }

    [Fact]
    public async Task ValidateAsync_ReturnBusinessFailure()
    {
        const string errorDetail = "errorDetail";
        const string errorName = "errorName";

        var request = new Request();
        var validationResult = new ValidationResult(
            new List<ValidationFailure>
            {
                new (errorName, errorDetail)
            });

        _mockedValidator.Setup(e => e.ValidateAsync(It.IsAny<Domain>(), default))
            .ReturnsAsync(validationResult)
            .Verifiable();

        var handlerResponse = await _handler.HandleAsync(request, default);

        handlerResponse.IsFailure.Should().BeTrue();
        handlerResponse.BusinessFailure.Errors
            .Should().ContainSingle(e => e.Name == errorName && e.Detail == errorDetail);
        handlerResponse.BusinessFailure
            .Kind.Should().Be(FailureKind.DomainValidation);

        _mockedValidator.Verify();
        _mockedValidator.VerifyNoOtherCalls();

        _mockedRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ValidateAsync_ReturnResponse()
    {
        var request = new Request();
        var domain = new Domain();

        _mockedValidator.Setup(e => e.ValidateAsync(It.IsAny<Domain>(), default))
            .ReturnsAsync(new ValidationResult())
            .Verifiable();

        _mockedRepository.Setup(e => e.RemoveAsync(It.IsAny<Domain>(), default))
            .ReturnsAsync(domain)
            .Verifiable();

        var handlerResponse = await _handler.HandleAsync(request, default);

        handlerResponse.IsSuccess.Should().BeTrue();

        _mockedValidator.Verify();
        _mockedValidator.VerifyNoOtherCalls();

        _mockedRepository.Verify();
        _mockedRepository.VerifyNoOtherCalls();
    }
}

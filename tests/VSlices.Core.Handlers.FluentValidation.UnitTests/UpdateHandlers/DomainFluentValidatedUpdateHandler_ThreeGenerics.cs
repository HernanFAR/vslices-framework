﻿using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using VSlices.Core.DataAccess.Abstracts;
using VSlices.Core.Abstracts.Responses;
using VSlices.Core.Abstracts.Requests;

namespace VSlices.Core.Handlers.FluentValidation.UnitTests.UpdateHandlers;


public class DomainFluentValidatedRemoveHandler_ThreeGenerics
{
    public record Domain;
    public record Response;
    public record Request : ICommand<Response>;

    public class EntityFluentValidatedUpdateHandler : EntityFluentValidatedUpdateHandler<Request, Response, Domain>
    {
        public EntityFluentValidatedUpdateHandler(IValidator<Domain> requestValidator, IUpdateRepository<Domain> repository) : base(requestValidator, repository) { }

        protected override ValueTask<Response<Success>> ValidateFeatureRulesAsync(Request request, CancellationToken cancellationToken = default)
            => ValueTask.FromResult<Response<Success>>(Success.Value);

        protected override ValueTask<Domain> GetAndProcessEntityAsync(Request request, CancellationToken cancellationToken = default)
            => ValueTask.FromResult(new Domain());

        protected override ValueTask<Response> GetResponseAsync(Domain domainEntity, Request request,
            CancellationToken cancellationToken) => ValueTask.FromResult(new Response());

    }

    private readonly Mock<IValidator<Domain>> _mockedValidator;
    private readonly Mock<IUpdateRepository<Domain>> _mockedRepository;
    private readonly EntityFluentValidatedUpdateHandler _handler;

    public DomainFluentValidatedRemoveHandler_ThreeGenerics()
    {
        _mockedValidator = new Mock<IValidator<Domain>>();
        _mockedRepository = new Mock<IUpdateRepository<Domain>>();
        _handler = new EntityFluentValidatedUpdateHandler(_mockedValidator.Object, _mockedRepository.Object);
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

        _mockedRepository.Setup(e => e.UpdateAsync(It.IsAny<Domain>(), default))
            .ReturnsAsync(domain)
            .Verifiable();

        var handlerResponse = await _handler.HandleAsync(request, default);

        handlerResponse.IsSuccess.Should().BeTrue();
        handlerResponse.SuccessValue
            .Should().BeOfType<Response>();

        _mockedValidator.Verify();
        _mockedValidator.VerifyNoOtherCalls();

        _mockedRepository.Verify();
        _mockedRepository.VerifyNoOtherCalls();
    }
}

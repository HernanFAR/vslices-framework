using System.Diagnostics;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.CrossCutting.Validation.FluentValidation.UnitTests;

public class FluentValidationBehaviorTests
{
    public record Request : IRequest<Success>;

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess()
    {
        var request = new Request();
        var response = Success.Value;

        var validator = Mock.Of<IValidator<Request>>();
        var validatorMock = Mock.Get(validator);

        validatorMock.Setup(e => e.ValidateAsync(request, default))
            .Returns(Task.FromResult(new ValidationResult()));

        var validationBehaviorMock = new Mock<FluentValidationBehavior<Request, Success>>(new List<IValidator<Request>> { validator });
        var validationBehavior = validationBehaviorMock.Object;

        var handlerResponse = await validationBehavior.HandleAsync(
            request, () => ValueTask.FromResult<Response<Success>>(response));

        handlerResponse.IsSuccess.Should().BeTrue();
        handlerResponse.SuccessValue.Should().Be(response);

    }

    [Fact]
    public async Task HandleAsync_ShouldReturnBusinessFailure_DetailHasValidator()
    {
        var request = new Request();
        var expMessage = "Error de ejemplo";

        var validator = Mock.Of<IValidator<Request>>();
        var validatorMock = Mock.Get(validator);

        validatorMock.Setup(e => e.ValidateAsync(request, default))
            .Returns(Task.FromResult(new ValidationResult(new[] { new ValidationFailure("", expMessage) })));

        var validationBehaviorMock = new Mock<FluentValidationBehavior<Request, Success>>(new List<IValidator<Request>> { validator }) { CallBase = true };
        var validationBehavior = validationBehaviorMock.Object;

        var handlerResponse = await validationBehavior.HandleAsync(request, () => throw new UnreachableException());

        handlerResponse.IsFailure.Should().BeTrue();
        handlerResponse.BusinessFailure.Kind.Should().Be(FailureKind.ContractValidation);
        handlerResponse.BusinessFailure.Errors.Should().ContainSingle(e => e == expMessage);

    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_DetailHasValidator()
    {
        var request = new Request();
        const string expMessage = "Error de ejemplo";

        var validator = Mock.Of<IValidator<Request>>();
        var validatorMock = Mock.Get(validator);

        validatorMock.Setup(e => e.ValidateAsync(request, default))
            .Returns(Task.FromResult(new ValidationResult()));

        var validationBehaviorMock = new Mock<FluentValidationBehavior<Request, Success>>(new List<IValidator<Request>> { validator }) { CallBase = true };
        var validationBehavior = validationBehaviorMock.Object;

        var handlerResponse = await validationBehavior.HandleAsync(
            request, 
            () => ValueTask.FromResult<Response<Success>>(Success.Value));

        handlerResponse.IsSuccess.Should().BeTrue();

    }

    [Fact]
    public async Task HandleAsync_ShouldReturnBusinessFailure_DetailHasNotValidator()
    {
        var request = new Request();

        var validationBehaviorMock = new Mock<FluentValidationBehavior<Request, Success>>(new List<IValidator<Request>>()) { CallBase = true };
        var validationBehavior = validationBehaviorMock.Object;

        var handlerResponse = await validationBehavior.HandleAsync(request, () => ValueTask.FromResult<Response<Success>>(Success.Value));

        handlerResponse.IsSuccess.Should().BeTrue();

    }
}
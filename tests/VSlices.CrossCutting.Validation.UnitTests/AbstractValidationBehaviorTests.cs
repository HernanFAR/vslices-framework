using FluentAssertions;
using Moq;
using VSlices.Core.Abstracts.Requests;
using VSlices.Core.Abstracts.Responses;

namespace VSlices.CrossCutting.Validation.UnitTests;

public class AbstractValidationBehaviorTests
{
    public record Request : IRequest<Success>;

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess()
    {
        var validationBehavior = Mock.Of<AbstractValidationBehavior<Request, Success>>();
        var validationBehaviorMock = Mock.Get(validationBehavior);

        var request = new Request();
        var response = Success.Value;

        validationBehaviorMock.Setup(e => e.ValidateAsync(request, default))
            .ReturnsAsync(response);

        var handlerResponse = await validationBehavior.HandleAsync(
            request,
            () => ValueTask.FromResult<Response<Success>>(response));

        handlerResponse.IsSuccess.Should().BeTrue();
        handlerResponse.SuccessValue.Should().Be(response);

    }

    [Fact]
    public async Task HandleAsync_ShouldReturnBusinessFailure()
    {
        var validationBehavior = Mock.Of<AbstractValidationBehavior<Request, Success>>();
        var validationBehaviorMock = Mock.Get(validationBehavior);

        var request = new Request();
        var response = BusinessFailure.Of.ContractValidation(
            error: new ValidationError("ErrorName", "ErrorDetail"));

        validationBehaviorMock.Setup(e => e.ValidateAsync(request, default))
            .ReturnsAsync(response);

        var handlerResponse = await validationBehavior.HandleAsync(request, () => throw new Exception());

        handlerResponse.IsFailure.Should().BeTrue();
        handlerResponse.BusinessFailure.Should().Be(response);

    }
}
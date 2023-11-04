using System.Runtime.InteropServices.ComTypes;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.Responses;
using VSlices.CrossCutting.Logging.Attributes;
using VSlices.CrossCutting.Logging.Configurations;
#pragma warning disable CS8602

namespace VSlices.CrossCutting.Logging.UnitTests;

public class LoggingBehaviorTests
{
    public record Request1 : IRequest<Response1>;

    public record Response1
    {
        public static string Test => "Testing";
    }

    [NoLoggable]
    public record Request2 : IRequest<Response2>;

    [NoLoggable]
    public record Response2
    {
        public static string Test => "Testing";
    }

    [Fact]
    public async Task Handle_ShouldLogSuccessInformation_DetailDoShowPropertyBecauseIsLoggeable()
    {
        // Arrange
        var logger = Mock.Of<ILogger<Request1>>();
        var configuration = new LoggingConfiguration();
        
        var request = new Request1();
        var response = new Response1();

        ValueTask<Response<Response1>> Next() => ValueTask.FromResult<Response<Response1>>(response);

        var loggingBehavior = new LoggingBehavior<Request1, Response1>(logger, configuration);


        // Act
        var handlerResponse = await loggingBehavior.HandleAsync(request, Next, CancellationToken.None);


        // Assert
        handlerResponse.IsSuccess.Should().BeTrue();

        var loggerMock = Mock.Get(logger);
        var jsonProperties = JsonSerializer.Serialize(request);

        // "Log Hour: {0} | Starting handling of {1}, with the following properties: {2}."
        loggerMock.Verify(
            e => e.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) =>
                    o.ToString()!.IndexOf("Log hour: ", StringComparison.Ordinal) != -1 &&
                    o.ToString()!.IndexOf($" | Starting handling of {typeof(Request1).FullName}, with the following properties: {jsonProperties}", StringComparison.Ordinal) != -1),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once);
        
        var responseProperties = JsonSerializer.Serialize(response, options: null);

        loggerMock.Verify(
            e => e.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) =>
                    o.ToString().IndexOf("Log hour: ", StringComparison.Ordinal) != -1 && 
                    o.ToString().IndexOf($" | Finishing handling of {typeof(Request1).FullName}, response obtained correctly: {responseProperties}.", StringComparison.Ordinal) != -1 
                    ),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once);

    }

    [Fact]
    public async Task Handle_ShouldLogSuccessInformation_DetailDoShowPropertyBecauseItNeedsToShowAll()
    {
        // Arrange
        var logger = Mock.Of<ILogger<Request2>>();
        var configuration = new LoggingConfiguration
        {
            SerializeAll = true
        };
        
        var request = new Request2();
        var response = new Response2();

        ValueTask<Response<Response2>> Next() => ValueTask.FromResult<Response<Response2>>(response);

        var loggingBehavior = new LoggingBehavior<Request2, Response2>(logger, configuration);


        // Act
        var handlerResponse = await loggingBehavior.HandleAsync(request, Next, CancellationToken.None);


        // Assert
        handlerResponse.IsSuccess.Should().BeTrue();

        var loggerMock = Mock.Get(logger);
        var jsonProperties = JsonSerializer.Serialize(request);
        
        loggerMock.Verify(
            e => e.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) =>
                    o.ToString()!.IndexOf("Log hour: ", StringComparison.Ordinal) != -1 &&
                    o.ToString()!.IndexOf($" | Starting handling of {typeof(Request2).FullName}, with the following properties: {jsonProperties}.", StringComparison.Ordinal) != -1
                    ),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once);


        var responseProperties = JsonSerializer.Serialize(response, options: null);

        loggerMock.Verify(
            e => e.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) =>
                    o.ToString().IndexOf("Log hour: ", StringComparison.Ordinal) != -1 && 
                    o.ToString().IndexOf($" | Finishing handling of {typeof(Request2).FullName}, response obtained correctly: {responseProperties}.", StringComparison.Ordinal) != -1
                    ),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once);

    }

    [Fact]
    public async Task Handle_ShouldLogSuccessInformation_DetailDoNotShowPropertyInfo()
    {
        // Arrange
        var logger = Mock.Of<ILogger<Request2>>();
        var configuration = new LoggingConfiguration();
        
        var request = new Request2();
        var response = new Response2();

        ValueTask<Response<Response2>> Next() => ValueTask.FromResult<Response<Response2>>(response);

        var loggingBehavior = new LoggingBehavior<Request2, Response2>(logger, configuration);


        // Act
        var handlerResponse = await loggingBehavior.HandleAsync(request, Next, CancellationToken.None);


        // Assert
        handlerResponse.IsSuccess.Should().BeTrue();

        var loggerMock = Mock.Get(logger);
        
        loggerMock.Verify(
            e => e.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) =>
                    o.ToString()!.IndexOf("Log hour: ", StringComparison.Ordinal) != -1 &&
                    o.ToString()!.IndexOf($" | Starting handling of {typeof(Request2).FullName}.", StringComparison.Ordinal) != -1),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once);

        loggerMock.Verify(
            e => e.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) =>
                    o.ToString().IndexOf("Log hour: ", StringComparison.Ordinal) != -1 && 
                    o.ToString().IndexOf($" | Finishing handling of {typeof(Request2).FullName}, response obtained correctly.", StringComparison.Ordinal) != -1),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once);

    }

    [Fact]
    public async Task Handle_ShouldLogFailureInformation_DetailDoShowPropertyBecauseIsLoggeable()
    {
        // Arrange
        var logger = Mock.Of<ILogger<Request1>>();
        var configuration = new LoggingConfiguration();
        
        var request = new Request1();
        var response = BusinessFailure.Of.Unspecified("Test");
        ValueTask<Response<Response1>> Next() => ValueTask.FromResult<Response<Response1>>(response);

        var loggingBehavior = new LoggingBehavior<Request1, Response1>(logger, configuration);


        // Act
        var handlerResponse = await loggingBehavior.HandleAsync(request, Next, CancellationToken.None);


        // Assert
        handlerResponse.BusinessFailure.Should().Be(response);

        var loggerMock = Mock.Get(logger);
        var jsonProperties = JsonSerializer.Serialize(request);
        
        loggerMock.Verify(
            e => e.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) =>
                    o.ToString()!.IndexOf("Log hour: ", StringComparison.Ordinal) != -1 &&
                    o.ToString()!.IndexOf($" | Starting handling of {typeof(Request1).FullName}, with the following properties: {jsonProperties}.", StringComparison.Ordinal) != -1),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once);

        var failureProperties = JsonSerializer.Serialize(response);

        loggerMock.Verify(
            e => e.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) =>
                    o.ToString().IndexOf("Log hour: ", StringComparison.Ordinal) != -1 && 
                    o.ToString().IndexOf($" | Finishing handling of {typeof(Request1).FullName}, response obtained with errors: {failureProperties}.", StringComparison.Ordinal) != -1
                    ),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once);

    }

    [Fact]
    public async Task Handle_ShouldLogFailureInformation_DetailDoShowPropertyBecauseItNeedsToShowAll()
    {
        // Arrange
        var logger = Mock.Of<ILogger<Request2>>();
        var configuration = new LoggingConfiguration
        {
            SerializeAll = true
        };
        
        var request = new Request2();
        var response = BusinessFailure.Of.Unspecified("Test");
        ValueTask<Response<Response2>> Next() => ValueTask.FromResult<Response<Response2>>(response);

        var loggingBehavior = new LoggingBehavior<Request2, Response2>(logger, configuration);


        // Act
        var handlerResponse = await loggingBehavior.HandleAsync(request, Next, CancellationToken.None);


        // Assert
        handlerResponse.BusinessFailure.Should().Be(response);

        var loggerMock = Mock.Get(logger);
        var jsonProperties = JsonSerializer.Serialize(request);
        
        loggerMock.Verify(
            e => e.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) =>
                    o.ToString()!.IndexOf("Log hour: ", StringComparison.Ordinal) != -1 &&
                    o.ToString()!.IndexOf($" | Starting handling of {typeof(Request2).FullName}, with the following properties: {jsonProperties}.", StringComparison.Ordinal) != -1),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once);


        var responseProperties = JsonSerializer.Serialize(response, options: null);

        loggerMock.Verify(
            e => e.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) =>
                    o.ToString().IndexOf("Log hour: ", StringComparison.Ordinal) != -1 && 
                    o.ToString().IndexOf($" | Finishing handling of {typeof(Request2).FullName}, response obtained with errors: {responseProperties}.", StringComparison.Ordinal) != -1
                    ),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ),
            Times.Once);

    }
}
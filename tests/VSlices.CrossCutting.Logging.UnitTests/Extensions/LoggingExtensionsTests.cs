using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using VSlices.Core.Abstracts.Handlers;
using VSlices.Core.Abstracts.Requests;
using VSlices.Core.Abstracts.Responses;
using VSlices.CrossCutting.Logging.Configurations;
// ReSharper disable UnassignedGetOnlyAutoProperty
#pragma warning disable CS8618

namespace VSlices.CrossCutting.Logging.UnitTests.Extensions;

public class LoggingExtensionsTests
{
    public class TestLoggingDescriber : ILoggingDescriber
    {
        public string Initial { get; }
        public string InitialWithoutProperties { get; }
        public string Success { get; }
        public string SuccessWithoutProperties { get; }
        public string Failure { get; }
        public string FailureWithoutProperties { get; }
    }

    public record Request : IRequest;

    public class LoggingBehavior2<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public ValueTask<Response<TResponse>> HandleAsync(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    [Fact]
    public void AddLoggingBehavior_ShouldAddLoggingBehaviorAndDefaultConfiguration_DetailWithoutConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();


        // Act
        services.AddLoggingBehavior();


        // Assert
        var provider = services.BuildServiceProvider();

        var loggingConfiguration = provider.GetRequiredService<LoggingConfiguration>();

        loggingConfiguration.SerializeAll.Should().BeFalse();
        loggingConfiguration.Describer.Should().BeOfType<DefaultLoggingDescriber>();
        loggingConfiguration.JsonOptions.Should().BeNull();

        services.Where(e => e.ServiceType == typeof(IPipelineBehavior<,>))
            .Any(e => e.ImplementationType == typeof(LoggingBehavior<,>))
            .Should().BeTrue();
    }

    [Fact]
    public void AddLoggingBehavior_ShouldAddLoggingBehaviorAndDefaultConfiguration_DetailWithConfigurationWithSpecificLoggingBehavior()
    {
        // Arrange
        var services = new ServiceCollection();
        var jsonOptions = new JsonSerializerOptions();

        // Act
        services.AddLoggingBehavior(opts =>
        {
            opts.Describer = new TestLoggingDescriber();
            opts.SerializeAll = true;
            opts.JsonOptions = jsonOptions;
        });


        // Assert
        var provider = services.BuildServiceProvider();

        var loggingConfiguration = provider.GetRequiredService<LoggingConfiguration>();

        loggingConfiguration.SerializeAll.Should().BeTrue();
        loggingConfiguration.Describer.Should().BeOfType<TestLoggingDescriber>();
        loggingConfiguration.JsonOptions.Should().Be(jsonOptions);

        services.Where(e => e.ServiceType == typeof(IPipelineBehavior<,>))
            .Any(e => e.ImplementationType == typeof(LoggingBehavior<,>))
            .Should().BeTrue();


    }

    [Fact]
    public void AddLoggingBehavior_ShouldAddLoggingBehaviorAndDefaultConfiguration_DetailWithoutConfigurationWithSpecificLoggingBehavior()
    {
        // Arrange
        var services = new ServiceCollection();


        // Act
        services.AddLoggingBehavior(typeof(LoggingBehavior2<,>));


        // Assert
        var provider = services.BuildServiceProvider();

        var loggingConfiguration = provider.GetRequiredService<LoggingConfiguration>();

        loggingConfiguration.SerializeAll.Should().BeFalse();
        loggingConfiguration.Describer.Should().BeOfType<DefaultLoggingDescriber>();
        loggingConfiguration.JsonOptions.Should().BeNull();

        services.Where(e => e.ServiceType == typeof(IPipelineBehavior<,>))
            .Any(e => e.ImplementationType == typeof(LoggingBehavior2<,>))
            .Should().BeTrue();
    }

    [Fact]
    public void AddLoggingBehavior_ShouldAddLoggingBehaviorAndDefaultConfiguration_DetailWithConfiguration()
    {
        // Arrange
        var services = new ServiceCollection();
        var jsonOptions = new JsonSerializerOptions();

        // Act
        services.AddLoggingBehavior(typeof(LoggingBehavior2<,>),
            opts =>
            {
                opts.Describer = new TestLoggingDescriber();
                opts.SerializeAll = true;
                opts.JsonOptions = jsonOptions;
            });


        // Assert
        var provider = services.BuildServiceProvider();

        var loggingConfiguration = provider.GetRequiredService<LoggingConfiguration>();

        loggingConfiguration.SerializeAll.Should().BeTrue();
        loggingConfiguration.Describer.Should().BeOfType<TestLoggingDescriber>();
        loggingConfiguration.JsonOptions.Should().Be(jsonOptions);

        services.Where(e => e.ServiceType == typeof(IPipelineBehavior<,>))
            .Any(e => e.ImplementationType == typeof(LoggingBehavior2<,>))
            .Should().BeTrue();


    }
}

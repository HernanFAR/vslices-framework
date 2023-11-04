using FluentValidation;
using Microsoft.Extensions.Hosting;
using VSlices.Core.Abstracts.Event;
using VSlices.Core.Abstracts.Handlers;
using VSlices.Core.Abstracts.Sender;
using VSlices.Core.Events.EventQueue.InMemory;
using VSlices.Core.Events.Publisher.Reflection;
using VSlices.Core.Presentation.AspNetCore;
using VSlices.Core.Sender.Reflection;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

#pragma warning disable CS1591
public static class DistributedAspNetFVEFReflectionMonolithExtensions
#pragma warning restore CS1591
{
    /// <summary>
    /// Shortcut to add all necessary services to the application, using the <typeparamref name="TAssembly"/> as anchor class.
    /// </summary>
    /// <remarks>
    /// It adds a:
    /// <list type="bullet">
    /// <item><see cref="ISender"/> implementation, <see cref="ReflectionSender" />.</item>
    /// <item><see cref="IPublisher"/> implementation, <see cref="ReflectionPublisher" />.</item>
    /// <item><see cref="IEventQueue"/>, <see cref="IEventQueueWriter"/> and <see cref="IEventQueueReader"/> implementations, as <see cref="InMemoryEventQueue" />.</item>
    /// <item>A <see cref="IHostedService"/> implementation, <see cref="BackgroundEventListenerService"/>.</item>
    /// <item>All <see cref="IHandler{TRequest}" /> implementations in <typeparamref name="TAssembly"/> assembly.</item>
    /// <item>All <see cref="ISimpleEndpointDefinition" /> implementations in <typeparamref name="TAssembly"/> assembly.</item>
    /// <item>All <see cref="IValidator{TRequest}" /> implementations in <typeparamref name="TAssembly"/> assembly.</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="TAssembly">Anchor class to get the endpoint definitions, handlers and validators</typeparam>
    /// <param name="services"></param>
    /// <returns>ServiceCollection</returns>
    public static IServiceCollection AddDistributedMonolithServicesAndHandlersFromAssemblyContaining<TAssembly>(this IServiceCollection services) 
        => services
            .AddReflectionSender()
            .AddReflectionPublisher()
            .AddInMemoryEventQueue()
            .AddBackgroundEventListenerService()
            .AddHandlersFromAssemblyContaining<TAssembly>()
            .AddEndpointDefinitionsFromAssemblyContaining<TAssembly>()
            .AddValidatorsFromAssemblyContaining<TAssembly>();
}

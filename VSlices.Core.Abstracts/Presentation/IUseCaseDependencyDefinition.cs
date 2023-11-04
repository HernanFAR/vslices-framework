using Microsoft.Extensions.DependencyInjection;

namespace VSlices.Core.Abstracts.Presentation;

/// <summary>
/// Specifies dependencies in a given use case
/// </summary>
public interface IUseCaseDependencyDefinition
{
    /// <summary>
    /// Defines the dependencies for the use case
    /// </summary>
    /// <param name="services">Service collection</param>
    static abstract void DefineDependencies(IServiceCollection services);
}

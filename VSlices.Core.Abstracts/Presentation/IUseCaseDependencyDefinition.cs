using Microsoft.Extensions.DependencyInjection;

namespace VSlices.Core.Abstracts.Presentation;

public interface IUseCaseDependencyDefinition
{
    static abstract void DefineDependencies(IServiceCollection services);
}

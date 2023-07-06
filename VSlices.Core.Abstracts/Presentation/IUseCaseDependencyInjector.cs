using Microsoft.Extensions.DependencyInjection;

namespace VSlices.Core.Abstracts.Presentation;

public interface IUseCaseDependencyInjector
{
    static abstract void DefineDependencies(IServiceCollection services);
}

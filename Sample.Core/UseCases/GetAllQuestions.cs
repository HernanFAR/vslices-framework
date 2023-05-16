using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sample.Core.Interfaces;
using Sample.Infrastructure.EntityFramework;

namespace Sample.Core.UseCases;

// Presentación
public class GetAllQuestionsEndpoint : IEndpointDefinition
{
    public void DefineEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/api/question", GetAllQuestionsAsync)
            .WithSwaggerOperationInfo("Obtiene preguntas", "Obtiene todas las preguntas")
            .Produces(StatusCodes.Status200OK);
    }

    public static void DefineDependencies(IServiceCollection services)
    {
        services.AddScoped<GetAllQuestionsHandler>();
        services.AddScoped<IGetAllQuestionsRepository, GetAllQuestionsRepository>();
    }

    public static async Task<IResult> GetAllQuestionsAsync(
        [FromServices] IHttpContextAccessor contextAccessor,
        [FromServices] GetAllQuestionsHandler handler,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(GetAllQuestionsQuery.Instance, cancellationToken);

        return Results.Ok(response);
    }
}

// Lógica
public record GetAllQuestionsDto(Guid Id, string Name, Guid CreatedById, Guid? UpdatedById);

public record GetAllQuestionsQuery
{
    private static readonly Lazy<GetAllQuestionsQuery> Lazy = new();
    public static GetAllQuestionsQuery Instance => Lazy.Value;
}

public class GetAllQuestionsHandler
{
    private readonly IGetAllQuestionsRepository _repository;

    public GetAllQuestionsHandler(IGetAllQuestionsRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetAllQuestionsDto[]> HandleAsync(GetAllQuestionsQuery _, CancellationToken cancellationToken)
    {
        return await _repository
            .GetAllQuestionsAsync(cancellationToken);
    }
}

public interface IGetAllQuestionsRepository
{
    Task<GetAllQuestionsDto[]> GetAllQuestionsAsync(CancellationToken cancellationToken = default);
}

public class GetAllQuestionsRepository : IGetAllQuestionsRepository
{
    private readonly ApplicationDbContext _context;

    public GetAllQuestionsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetAllQuestionsDto[]> GetAllQuestionsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Questions
            .Select(e => new GetAllQuestionsDto(e.Id, e.Name, e.CreatedById, e.UpdatedById))
            .ToArrayAsync(cancellationToken);
    }
}
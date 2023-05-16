using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sample.Core.Interfaces;
using Sample.Infrastructure.EntityFramework;

namespace Sample.Core.UseCases;

// Presentación
public class GetQuestionEndpoint : IEndpointDefinition
{
    public void DefineEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/api/question/{id}", GetQuestionAsync)
            .WithSwaggerOperationInfo("Obtiene una pregunta", "Obtiene una preguntas en base al Id")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    public static void DefineDependencies(IServiceCollection services)
    {
        services.AddScoped<GetQuestionHandler>();
        services.AddScoped<IGetQuestionRepository, GetQuestionRepository>();
    }

    public static async Task<IResult> GetQuestionAsync(
        [FromRoute] string id,
        [FromServices] IHttpContextAccessor contextAccessor,
        [FromServices] GetQuestionHandler handler,
        CancellationToken cancellationToken)
    {
        _ = Guid.TryParse(id, out var questionId);

        var response = await handler.HandleAsync(new GetQuestionQuery(questionId), cancellationToken);

        return response.Match<IResult>(
            e => TypedResults.Ok(e),
            _ => TypedResults.NotFound());
    }
}

// Lógica
public record GetQuestionDto(Guid Id, string Name, Guid CreatedById, Guid? UpdatedById);

public record GetQuestionQuery(Guid Id);

public class GetQuestionHandler
{
    private readonly IGetQuestionRepository _repository;

    public GetQuestionHandler(IGetQuestionRepository repository)
    {
        _repository = repository;
    }

    public async Task<OneOf<GetQuestionDto, NotFound>> HandleAsync(GetQuestionQuery request, CancellationToken cancellationToken)
    {
        var question = await _repository
            .GetQuestionAsync(request.Id, cancellationToken);

        return question is not null ?
            question :
            new NotFound();
    }
}

public interface IGetQuestionRepository
{
    Task<GetQuestionDto?> GetQuestionAsync(Guid id, CancellationToken cancellationToken = default);
}

public class GetQuestionRepository : IGetQuestionRepository
{
    private readonly ApplicationDbContext _context;

    public GetQuestionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetQuestionDto?> GetQuestionAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Questions
            .Where(e => e.Id == id)
            .Select(e => new GetQuestionDto(e.Id, e.Name, e.CreatedById, e.UpdatedById))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
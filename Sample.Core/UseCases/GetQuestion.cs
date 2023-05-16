using Microsoft.EntityFrameworkCore;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Presentation;
using VSlices.Core.Abstracts.Responses;

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
            e =>
            {
                return e.Kind switch
                {
                    FailureKind.UserNotAllowed => TypedResults.Forbid(),
                    FailureKind.NotFoundResource => TypedResults.NotFound(),
                    FailureKind.ConcurrencyError => TypedResults.Conflict(),
                    FailureKind.Validation => TypedResults.UnprocessableEntity(e.Errors),
                    _ => throw new ArgumentOutOfRangeException(nameof(e.Kind), "A not valid FailureKind value was returned")
                };
            });
    }
}

// Lógica
public record GetQuestionDto(Guid Id, string Name, Guid CreatedById, Guid? UpdatedById);

public record GetQuestionQuery(Guid Id);

public class GetQuestionHandler : IHandler<GetQuestionQuery, GetQuestionDto>
{
    private readonly IGetQuestionRepository _repository;

    public GetQuestionHandler(IGetQuestionRepository repository)
    {
        _repository = repository;
    }

    public async Task<OneOf<GetQuestionDto, BusinessFailure>> HandleAsync(GetQuestionQuery request, CancellationToken cancellationToken)
    {
        return await _repository.ReadAsync(request.Id, cancellationToken);
    }
}

public interface IGetQuestionRepository : IReadableRepository<GetQuestionDto, Guid>
{

}

public class GetQuestionRepository : IGetQuestionRepository
{
    private readonly ApplicationDbContext _context;

    public GetQuestionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OneOf<GetQuestionDto, BusinessFailure>> ReadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var question = await _context.Questions
            .Where(e => e.Id == id)
            .Select(e => new GetQuestionDto(e.Id, e.Name, e.CreatedById, e.UpdatedById))
            .FirstOrDefaultAsync(cancellationToken);

        return question is not null
            ? question
            : BusinessFailure.Of.NotFoundResource(Array.Empty<string>());
    }
}
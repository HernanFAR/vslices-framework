using Microsoft.EntityFrameworkCore;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Presentation;
using VSlices.Core.Abstracts.Responses;
using VSlices.Core.BusinessLogic;

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

    public static async ValueTask<IResult> GetQuestionAsync(
        [FromRoute] string id,
        [FromServices] IHttpContextAccessor contextAccessor,
        [FromServices] GetQuestionHandler handler,
        CancellationToken cancellationToken)
    {
        _ = Guid.TryParse(id, out var questionId);

        var response = await handler.HandleAsync(new GetQuestionQuery(questionId), cancellationToken);

        return response.MatchEndpointResult(TypedResults.Ok);
    }
}

// Lógica
public record GetQuestionDto(Guid Id, string Name, Guid CreatedById, Guid? UpdatedById);

public record GetQuestionQuery(Guid Id);

public class GetQuestionHandler : RequestValidatedReadHandler<GetQuestionQuery, Guid, GetQuestionDto>
{
    public GetQuestionHandler(IGetQuestionRepository repository)
        : base(repository) { }

    protected override ValueTask<OneOf<Success, BusinessFailure>> ValidateRequestAsync(GetQuestionQuery request, CancellationToken cancellationToken)
        => ValueTask.FromResult<OneOf<Success, BusinessFailure>>(new Success());

    protected override ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(GetQuestionQuery request, CancellationToken cancellationToken)
        => ValueTask.FromResult<OneOf<Success, BusinessFailure>>(new Success());

    protected override ValueTask<Guid> RequestToSearchOptionsAsync(GetQuestionQuery request, CancellationToken cancellationToken)
        => ValueTask.FromResult(request.Id);
}

public interface IGetQuestionRepository : IReadRepository<GetQuestionDto, Guid>
{

}

public class GetQuestionRepository : IGetQuestionRepository
{
    private readonly ApplicationDbContext _context;

    public GetQuestionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<OneOf<GetQuestionDto, BusinessFailure>> ReadAsync(Guid id, CancellationToken cancellationToken = default)
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
using Microsoft.EntityFrameworkCore;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Presentation;
using VSlices.Core.Abstracts.Responses;
using VSlices.Core.BusinessLogic;

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
public record GetAllQuestionsDto(Guid Id, string Name, Guid CreatedById, Guid? UpdatedById);

public record GetAllQuestionsQuery
{
    private static readonly Lazy<GetAllQuestionsQuery> Lazy = new();
    public static GetAllQuestionsQuery Instance => Lazy.Value;
}

public class GetAllQuestionsHandler : AbstractBasicReadRequestValidatedHandler<GetAllQuestionsQuery, GetAllQuestionsDto[]>
{
    public GetAllQuestionsHandler(IGetAllQuestionsRepository repository) : base(repository) { }

    protected override async Task<OneOf<Success, BusinessFailure>> ValidateRequestAsync(GetAllQuestionsQuery request, CancellationToken cancellationToken)
        => new Success();

    protected override async Task<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(GetAllQuestionsQuery request, CancellationToken cancellationToken)
        => new Success();
}

public interface IGetAllQuestionsRepository : IReadableRepository<GetAllQuestionsDto[]>
{
}

public class GetAllQuestionsRepository : IGetAllQuestionsRepository
{
    private readonly ApplicationDbContext _context;

    public GetAllQuestionsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OneOf<GetAllQuestionsDto[], BusinessFailure>> ReadAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Questions
            .Select(e => new GetAllQuestionsDto(e.Id, e.Name, e.CreatedById, e.UpdatedById))
            .ToArrayAsync(cancellationToken);
    }
}
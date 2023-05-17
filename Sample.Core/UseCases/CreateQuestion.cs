using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Presentation;
using VSlices.Core.Abstracts.Responses;
using VSlices.Core.BusinessLogic;
using VSlices.Core.BusinessLogic.FluentValidation;

namespace Sample.Core.UseCases;

// Presentación
public record CreateQuestionContract(string Name);

public class CreateQuestionEndpoint : IEndpointDefinition
{
    public void DefineEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/question", CreateQuestionAsync)
            .WithSwaggerOperationInfo("Crea una pregunta", "Crea una pregunta, en base al body")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status422UnprocessableEntity);

    }

    public static void DefineDependencies(IServiceCollection services)
    {
        services.AddScoped<CreateQuestionHandler>();
        services.AddScoped<ICreateQuestionRepository, CreateQuestionRepository>();
    }

    public static async Task<IResult> CreateQuestionAsync(
        [FromBody] CreateQuestionContract createQuestionContract,
        [FromServices] IHttpContextAccessor contextAccessor,
        [FromServices] CreateQuestionHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateQuestionCommand(
            createQuestionContract.Name,
            contextAccessor.GetIdentityId());

        var response = await handler.HandleAsync(command, cancellationToken);

        return response.Match<IResult>(
                e => TypedResults.Created($"/api/question/{e}"),
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
public record CreateQuestionCommand(string Name, Guid CreatedBy);

public class CreateQuestionHandler : AbstractCreateFullyFluentValidatedHandler<CreateQuestionCommand, Guid, Question>
{ 
    public CreateQuestionHandler(
        IValidator<CreateQuestionCommand> requestValidator,
        IValidator<Question> domainValidator,
        ICreateQuestionRepository repository) : base(requestValidator, domainValidator, repository)
    { }

    protected override async Task<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(CreateQuestionCommand request, CancellationToken cancellationToken)
        => new Success();

    protected override async Task<Question> GetDomainEntityAsync(CreateQuestionCommand request, CancellationToken cancellationToken)
        => new Question(request.Name, request.CreatedBy);

    protected override async Task<Guid> GetResponseAsync(Question domainEntity, CreateQuestionCommand request, CancellationToken cancellationToken)
        => domainEntity.Id;
}

public class CreateQuestionValidator : AbstractValidator<CreateQuestionCommand>
{
    public CreateQuestionValidator()
    {
        RuleFor(e => e.Name)
            .NotEmpty();

        RuleFor(e => e.CreatedBy)
            .NotEmpty();

    }
}

public interface ICreateQuestionRepository : ICreatableRepository<Question>
{
}

public class CreateQuestionRepository : ICreateQuestionRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CreateQuestionRepository> _logger;

    public CreateQuestionRepository(ApplicationDbContext context, ILogger<CreateQuestionRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<OneOf<Success, BusinessFailure>> CreateAsync(Question question, CancellationToken cancellationToken = default)
    {
        _context.Add(question);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);

            return new Success();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Hubo un error de concurrencia al momento de actualizar la entidad {Entity}",
                JsonSerializer.Serialize(question));

            return BusinessFailure.Of.ConcurrencyError(Array.Empty<string>());
        }
    }
}
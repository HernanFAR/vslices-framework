using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sample.Domain;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Presentation;
using VSlices.Core.Abstracts.Responses;
using VSlices.Core.BusinessLogic;

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

public class CreateQuestionHandler : AbstractCreateFullyValidatedHandler<CreateQuestionCommand, Guid, Question>
{
    private readonly IValidator<CreateQuestionCommand> _contractValidator;
    private readonly IValidator<Question> _domainValidator;

    public CreateQuestionHandler(ICreateQuestionRepository repository,
        IValidator<CreateQuestionCommand> contractValidator,
        IValidator<Question> domainValidator) : base(repository)
    {
        _contractValidator = contractValidator;
        _domainValidator = domainValidator;
    }

    protected override async Task<OneOf<Success, BusinessFailure>> ValidateRequestAsync(CreateQuestionCommand request, CancellationToken cancellationToken = default)
    {
        var contractValidationResult = await _contractValidator.ValidateAsync(request, cancellationToken);

        if (contractValidationResult.IsValid) return new Success();

        var errors = contractValidationResult
            .Errors.Select(e => e.ErrorMessage)
            .ToArray();

        return BusinessFailure.Of.Validation(errors);

    }

    protected override async Task<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(CreateQuestionCommand request, CancellationToken cancellationToken) 
        => new Success();

    protected override async Task<Question> GetDomainEntityAsync(CreateQuestionCommand request, CancellationToken cancellationToken) 
        => new Question(request.Name, request.CreatedBy);

    protected override async Task<OneOf<Success, BusinessFailure>> ValidateDomainAsync(Question domain, CancellationToken cancellationToken = default)
    {
        var domainValidationResult = await _domainValidator.ValidateAsync(domain, cancellationToken);

        if (domainValidationResult.IsValid) return new Success();
        var errors = domainValidationResult
            .Errors.Select(e => e.ErrorMessage)
            .ToArray();

        return BusinessFailure.Of.Validation(errors);

    }

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
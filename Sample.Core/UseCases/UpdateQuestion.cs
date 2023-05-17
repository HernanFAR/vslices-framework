using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using VSlices.Core.Abstracts.BusinessLogic;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Presentation;
using VSlices.Core.Abstracts.Responses;
using VSlices.Core.BusinessLogic;

namespace Sample.Core.UseCases;

// Presentación
public record UpdateQuestionContract(string Name);

public class UpdateQuestionEndpoint : IEndpointDefinition
{
    public void DefineEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPatch("/api/question/{id}", UpdateQuestionAsync)
            .WithSwaggerOperationInfo("Actualiza una pregunta", "Actualiza una pregunta, en base al identificador y al body")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status422UnprocessableEntity);
    }

    public static void DefineDependencies(IServiceCollection services)
    {
        services.AddScoped<UpdateQuestionHandler>();
        services.AddScoped<IUpdateQuestionRepository, UpdateQuestionRepository>();
    }

    public static async Task<IResult> UpdateQuestionAsync(
        [FromRoute] string id,
        [FromBody] UpdateQuestionContract createQuestionContract,
        [FromServices] IHttpContextAccessor contextAccessor,
        [FromServices] UpdateQuestionHandler handler,
        CancellationToken cancellationToken)
    {
        _ = Guid.TryParse(id, out var questionId);

        var command = new UpdateQuestionCommand(
            questionId,
            createQuestionContract.Name,
            contextAccessor.GetIdentityId());

        var response = await handler.HandleAsync(command, cancellationToken);

        return response.Match(
                e => Results.NoContent(),
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
public record UpdateQuestionCommand(Guid Id, string Name, Guid UpdatedById);

public class UpdateQuestionHandler : AbstractUpdateFullyValidatedHandler<UpdateQuestionCommand, Success, Question>
{
    private readonly IUpdateQuestionRepository _repository;
    private readonly IValidator<UpdateQuestionCommand> _contractValidator;
    private readonly IValidator<Question> _domainValidator;

    public UpdateQuestionHandler(IUpdateQuestionRepository repository,
        IValidator<UpdateQuestionCommand> contractValidator,
        IValidator<Question> domainValidator) : base(repository)
    {
        _repository = repository;
        _contractValidator = contractValidator;
        _domainValidator = domainValidator;
    }
    
    protected override async Task<OneOf<Success, BusinessFailure>> ValidateRequestAsync(UpdateQuestionCommand request, CancellationToken cancellationToken = default)
    {
        var contractValidationResult = await _contractValidator.ValidateAsync(request, cancellationToken);

        if (contractValidationResult.IsValid) return new Success();

        var errors = contractValidationResult
            .Errors.Select(e => e.ErrorMessage)
            .ToArray();

        return BusinessFailure.Of.Validation(errors);

    }

    protected override async Task<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        var existQuestion = await _repository.AnyAsync(request.Id, cancellationToken);

        if (!existQuestion)
        {
            return BusinessFailure.Of.NotFoundResource(Array.Empty<string>());
        }

        return new Success();
    }

    protected override async Task<Question> GetDomainEntityAsync(UpdateQuestionCommand request, CancellationToken cancellationToken) => 
        await _repository.GetAsync(request.Id, cancellationToken);

    protected override async Task<OneOf<Success, BusinessFailure>> ValidateDomainAsync(Question domain, CancellationToken cancellationToken = default)
    {
        var domainValidationResult = await _domainValidator.ValidateAsync(domain, cancellationToken);

        if (domainValidationResult.IsValid) return new Success();

        var errors = domainValidationResult
            .Errors.Select(e => e.ErrorMessage)
            .ToArray();

        return BusinessFailure.Of.Validation(errors);

    }

    protected override async Task<Success> GetResponseAsync(Question domainEntity, UpdateQuestionCommand request, CancellationToken cancellationToken) 
        => new Success();
}

public class UpdateQuestionValidator : AbstractValidator<UpdateQuestionCommand>
{
    public UpdateQuestionValidator()
    {
        RuleFor(e => e.Name)
            .NotEmpty();

        RuleFor(e => e.UpdatedById)
            .NotEmpty();

    }
}

public interface IUpdateQuestionRepository : IUpdateableRepository<Question>
{
    Task<bool> AnyAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Question> GetAsync(Guid id, CancellationToken cancellationToken = default);


}

public class UpdateQuestionRepository : IUpdateQuestionRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UpdateQuestionRepository> _logger;

    public UpdateQuestionRepository(ApplicationDbContext context, ILogger<UpdateQuestionRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> AnyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Questions
            .AnyAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<Question> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Questions
            .FirstAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<OneOf<Success, BusinessFailure>> UpdateAsync(Question question,
        CancellationToken cancellationToken = default)
    {
        _context.Update(question);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);

            return new Success();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Hubo un error de concurrencia al momento de eliminar la entidad {Entity}",
                JsonSerializer.Serialize(question));

            return BusinessFailure.Of.ConcurrencyError(Array.Empty<string>());
        }
    }
}
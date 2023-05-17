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
public class RemoveQuestionEndpoint : IEndpointDefinition
{
    public void DefineEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapDelete("/api/question/{id}", RemoveQuestionAsync)
            .WithSwaggerOperationInfo("Elimina pregunta", "Elimina una pregunta, en base a su identificador")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status422UnprocessableEntity);

    }

    public static void DefineDependencies(IServiceCollection services)
    {
        services.AddScoped<RemoveQuestionHandler>();
        services.AddScoped<IRemoveQuestionRepository, RemoveQuestionRepository>();
    }

    public static async Task<IResult> RemoveQuestionAsync(
        [FromRoute] string id,
        [FromServices] IHttpContextAccessor contextAccessor,
        [FromServices] RemoveQuestionHandler handler,
        CancellationToken cancellationToken)
    {
        _ = Guid.TryParse(id, out var questionId);

        var command = new RemoveQuestionCommand(
            questionId,
            contextAccessor.GetIdentityId());

        var response = await handler.HandleAsync(command, cancellationToken);

        return response.Match(
                _ => Results.NoContent(),
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
public record RemoveQuestionCommand(Guid Id, Guid RemovedById);

public class RemoveQuestionHandler : AbstractRemoveFullyValidatedHandler<RemoveQuestionCommand, Success, Question>
{
    private readonly IRemoveQuestionRepository _repository;
    private readonly IValidator<RemoveQuestionCommand> _contractValidator;
    private readonly IValidator<Question> _domainValidator;

    public RemoveQuestionHandler(IRemoveQuestionRepository repository,
        IValidator<RemoveQuestionCommand> contractValidator,
        IValidator<Question> domainValidator) : base(repository)
    {
        _repository = repository;
        _contractValidator = contractValidator;
        _domainValidator = domainValidator;
    }

    protected override async Task<OneOf<Success, BusinessFailure>> ValidateRequestAsync(RemoveQuestionCommand request, CancellationToken cancellationToken = default)
    {
        var contractValidationResult = await _contractValidator.ValidateAsync(request, cancellationToken);

        if (contractValidationResult.IsValid) return new Success();

        var errors = contractValidationResult
            .Errors.Select(e => e.ErrorMessage)
            .ToArray();

        return BusinessFailure.Of.Validation(errors);

    }

    protected override async Task<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(RemoveQuestionCommand request, CancellationToken cancellationToken)
    {
        var exist = await _repository.AnyAsync(request.Id, cancellationToken);

        if (!exist)
        {
            return BusinessFailure.Of.NotFoundResource();
        }

        return new Success();
    }

    protected override async Task<Question> GetDomainEntityAsync(RemoveQuestionCommand request, CancellationToken cancellationToken) 
        => await _repository.GetAsync(request.Id, cancellationToken);

    protected override async Task<OneOf<Success, BusinessFailure>> ValidateDomainAsync(Question domain, CancellationToken cancellationToken = default)
    {
        var domainValidationResult = await _domainValidator.ValidateAsync(domain, cancellationToken);

        if (domainValidationResult.IsValid) return new Success();
        var errors = domainValidationResult
            .Errors.Select(e => e.ErrorMessage)
            .ToArray();

        return BusinessFailure.Of.Validation(errors);

    }

    protected override async Task<Success> GetResponseAsync(Question domainEntity, RemoveQuestionCommand request, CancellationToken cancellationToken) 
        => new Success();
}

public class RemoveQuestionValidator : AbstractValidator<RemoveQuestionCommand>
{
    public RemoveQuestionValidator()
    {
        RuleFor(e => e.RemovedById)
            .NotEmpty();

    }
}

public interface IRemoveQuestionRepository : IRemovableRepository<Question>
{
    Task<bool> AnyAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Question> GetAsync(Guid id, CancellationToken cancellationToken = default);

}

public class RemoveQuestionRepository : IRemoveQuestionRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RemoveQuestionRepository> _logger;

    public RemoveQuestionRepository(ApplicationDbContext context, ILogger<RemoveQuestionRepository> logger)
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

    public async Task<OneOf<Success, BusinessFailure>> RemoveAsync(Question question, CancellationToken cancellationToken = default)
    {
        _context.Remove(question);

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
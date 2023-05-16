using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sample.Core.Extensions;
using Sample.Core.Interfaces;
using Sample.Domain;
using Sample.Infrastructure.EntityFramework;

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

        return response
            .Match(
                e => Results.NoContent(),
                e => Results.NotFound(),
                e => Results.UnprocessableEntity(e.Value));
    }
}

// Lógica
public record RemoveQuestionCommand(Guid Id, Guid RemovedById);

public class RemoveQuestionHandler
{
    private readonly IRemoveQuestionRepository _repository;
    private readonly IValidator<RemoveQuestionCommand> _contractValidator;
    private readonly IValidator<Question> _domainValidator;

    public RemoveQuestionHandler(IRemoveQuestionRepository repository,
        IValidator<RemoveQuestionCommand> contractValidator,
        IValidator<Question> domainValidator)
    {
        _repository = repository;
        _contractValidator = contractValidator;
        _domainValidator = domainValidator;
    }

    public async Task<OneOf<Success, NotFound, Error<string[]>>> HandleAsync(RemoveQuestionCommand request, CancellationToken cancellationToken)
    {
        var contractValidationResult = await _contractValidator.ValidateAsync(request, cancellationToken);

        if (!contractValidationResult.IsValid)
        {
            return new Error<string[]>(contractValidationResult
                .Errors.Select(e => e.ErrorMessage)
                .ToArray());
        }

        var question = await _repository.GetQuestion(request.Id, cancellationToken);

        if (question is null)
        {
            return new NotFound();
        }

        var domainValidationResult = await _domainValidator.ValidateAsync(question, cancellationToken);

        if (!domainValidationResult.IsValid)
        {
            return new Error<string[]>(domainValidationResult
                .Errors.Select(e => e.ErrorMessage)
                .ToArray());
        }

        await _repository.RemoveQuestion(question, cancellationToken);

        return new Success();
    }
}

public class RemoveQuestionValidator : AbstractValidator<RemoveQuestionCommand>
{
    public RemoveQuestionValidator()
    {
        RuleFor(e => e.RemovedById)
            .NotEmpty();

    }
}

public interface IRemoveQuestionRepository
{
    Task<Question?> GetQuestion(Guid id, CancellationToken cancellationToken = default);

    Task RemoveQuestion(Question question, CancellationToken cancellationToken = default);

}

public class RemoveQuestionRepository : IRemoveQuestionRepository
{
    private readonly ApplicationDbContext _context;

    public RemoveQuestionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Question?> GetQuestion(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Questions
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task RemoveQuestion(Question question, CancellationToken cancellationToken = default)
    {
        _context.Remove(question);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
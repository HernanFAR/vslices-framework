using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Presentation;
using VSlices.Core.Abstracts.Responses;
using VSlices.Core.BusinessLogic.FluentValidation;
using VSlices.Core.DataAccess;

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

        return response.MatchEndpointResult(_ => Results.NoContent());
    }
}

// Lógica
public record UpdateQuestionCommand(Guid Id, string Name, Guid UpdatedById);

public class UpdateQuestionHandler : AbstractUpdateFullyFluentValidatedHandler<UpdateQuestionCommand, Question>
{
    private readonly IUpdateQuestionRepository _repository;

    public UpdateQuestionHandler(
        IValidator<UpdateQuestionCommand> requestValidator,
        IValidator<Question> domainValidator,
        IUpdateQuestionRepository repository) : base(requestValidator, domainValidator, repository)
    {
        _repository = repository;
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

    protected override async Task<Question> GetDomainEntityAsync(UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        var question = await _repository.GetAsync(request.Id, cancellationToken);

        question.UpdateState(request.Name, request.UpdatedById);

        return question;
    }
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

public class UpdateQuestionRepository : EFUpdateableRepository<ApplicationDbContext, Question>, IUpdateQuestionRepository
{
    private readonly ApplicationDbContext _context;

    public UpdateQuestionRepository(ApplicationDbContext context, ILogger<UpdateQuestionRepository> logger)
        : base(context, logger)
    {
        _context = context;
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
}
using Microsoft.Extensions.Logging;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Presentation;
using VSlices.Core.Abstracts.Responses;
using VSlices.Core.BusinessLogic.FluentValidation;
using VSlices.Core.DataAccess;
using VSlices.Core.DataAccess.EntityFramework;

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

    public static async ValueTask<IResult> CreateQuestionAsync(
        [FromBody] CreateQuestionContract createQuestionContract,
        [FromServices] IHttpContextAccessor contextAccessor,
        [FromServices] CreateQuestionHandler handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateQuestionCommand(
            createQuestionContract.Name,
            contextAccessor.GetIdentityId());

        var response = await handler.HandleAsync(command, cancellationToken);

        return response.MatchEndpointResult(e => TypedResults.Created($"/api/question/{e}"));
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

    protected override ValueTask<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(CreateQuestionCommand request, CancellationToken cancellationToken = default)
        => ValueTask.FromResult<OneOf<Success, BusinessFailure>>(new Success());

    protected override ValueTask<Question> GetDomainEntityAsync(CreateQuestionCommand request, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(new Question(request.Name, request.CreatedBy));

    protected override ValueTask<Guid> GetResponseAsync(Question domainEntity, CreateQuestionCommand request, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(domainEntity.Id);
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

public class CreateQuestionRepository : EFCreatableRepository<ApplicationDbContext, Question>, ICreateQuestionRepository
{
    public CreateQuestionRepository(ApplicationDbContext context, ILogger<CreateQuestionRepository> logger)
        : base(context, logger) { }
}
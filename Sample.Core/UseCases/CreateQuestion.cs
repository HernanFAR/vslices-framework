using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using VSlices.Core.Abstracts.DataAccess;
using VSlices.Core.Abstracts.Presentation;
using VSlices.Core.Abstracts.Responses;
using VSlices.Core.BusinessLogic;
using VSlices.Core.BusinessLogic.FluentValidation;
using VSlices.Core.DataAccess;

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

    protected override async Task<OneOf<Success, BusinessFailure>> ValidateUseCaseRulesAsync(CreateQuestionCommand request, CancellationToken cancellationToken = default)
        => new Success();

    protected override async Task<Question> GetDomainEntityAsync(CreateQuestionCommand request, CancellationToken cancellationToken = default)
        => new Question(request.Name, request.CreatedBy);

    protected override async Task<Guid> GetResponseAsync(Question domainEntity, CreateQuestionCommand request, CancellationToken cancellationToken = default)
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

public class CreateQuestionRepository : EFCreatableRepository<ApplicationDbContext, Question>, ICreateQuestionRepository
{
    public CreateQuestionRepository(ApplicationDbContext context, ILogger<CreateQuestionRepository> logger) 
        : base(context, logger) { }
}
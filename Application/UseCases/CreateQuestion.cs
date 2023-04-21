namespace Core.UseCases;

// Presentación
public record CreateQuestionContract(string Name);

public class CreateQuestionEndpoint : IEndpointDefinition
{
    public const string Uri = "/api/question";

    public void DefineEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapPost("/api/question", CreateQuestionAsync)
            .WithSwaggerOperationInfo("Crea una pregunta", "Crea una pregunta, en base al body")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status422UnprocessableEntity);

    }

    public static async Task<IResult> CreateQuestionAsync(
        [FromBody] CreateQuestionContract createQuestionContract,
        [FromServices] IHttpContextAccessor contextAccessor,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreateQuestionCommand(
            createQuestionContract.Name,
            contextAccessor.GetIdentityId());

        var response = await sender.Send(command, cancellationToken);

        return response
            .Match<IResult>(
                e => TypedResults.Created($"/api/question/{e}"),
                e => TypedResults.UnprocessableEntity(e.Value));
    }

}

// Lógica
public record CreateQuestionCommand(string Name, Guid CreatedBy) : IRequest<OneOf<Guid, Error<string[]>>>;

public class CreateQuestionHandler : IRequestHandler<CreateQuestionCommand, OneOf<Guid, Error<string[]>>>
{
    private readonly ICreateQuestionRepository _repository;
    private readonly IValidator<CreateQuestionCommand> _contractValidator;
    private readonly IValidator<Question> _domainValidator;

    public CreateQuestionHandler(ICreateQuestionRepository repository,
        IValidator<CreateQuestionCommand> contractValidator,
        IValidator<Question> domainValidator)
    {
        _repository = repository;
        _contractValidator = contractValidator;
        _domainValidator = domainValidator;
    }

    public async Task<OneOf<Guid, Error<string[]>>> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        var contractValidationResult = await _contractValidator.ValidateAsync(request, cancellationToken);

        if (!contractValidationResult.IsValid)
        {
            return new Error<string[]>(contractValidationResult
                .Errors.Select(e => e.ErrorMessage)
                .ToArray());
        }

        var question = new Question(request.Name, request.CreatedBy);

        var domainValidationResult = await _domainValidator.ValidateAsync(question, cancellationToken);

        if (!domainValidationResult.IsValid)
        {
            return new Error<string[]>(domainValidationResult
                .Errors.Select(e => e.ErrorMessage)
                .ToArray());
        }

        await _repository.SaveQuestion(question, cancellationToken);

        return question.Id;
    }
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

public interface ICreateQuestionRepository
{
    Task SaveQuestion(Question question, CancellationToken cancellationToken = default);

}

namespace Core.UseCases;

// Presentación
public class GetQuestionEndpoint : IEndpointDefinition
{
    public void DefineEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/api/question/{id}", GetQuestionAsync)
            .WithSwaggerOperationInfo("Obtiene una pregunta", "Obtiene una preguntas en base al Id")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    public static async Task<IResult> GetQuestionAsync(
        [FromRoute] string id,
        [FromServices] IHttpContextAccessor contextAccessor,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        _ = Guid.TryParse(id, out var questionId);

        var response = await sender.Send(new GetQuestionQuery(questionId), cancellationToken);

        return response
            .Match(
                e => Results.Ok(e),
                _ => Results.NotFound());
    }
}

// Lógica
public record GetQuestionDto(Guid Id, string Name, Guid CreatedById, Guid? UpdatedById);

public record GetQuestionQuery(Guid Id) : IRequest<OneOf<GetQuestionDto, NotFound>>;

public class GetQuestionHandler : IRequestHandler<GetQuestionQuery, OneOf<GetQuestionDto, NotFound>>
{
    private readonly IGetQuestionRepository _repository;

    public GetQuestionHandler(IGetQuestionRepository repository)
    {
        _repository = repository;
    }

    public async Task<OneOf<GetQuestionDto, NotFound>> Handle(GetQuestionQuery request, CancellationToken cancellationToken)
    {
        var question = await _repository
            .GetQuestionAsync(request.Id, cancellationToken);

        return question is not null ?
            question :
            new NotFound();
    }
}

public interface IGetQuestionRepository
{
    Task<GetQuestionDto?> GetQuestionAsync(Guid id, CancellationToken cancellationToken = default);
}
namespace Application.UseCases;

public class GetAllQuestionsEndpoint : IEndpointDefinition
{
    public void DefineEndpoint(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/api/question", GetAllQuestionsAsync)
            .WithSwaggerOperationInfo("Obtiene preguntas", "Obtiene todas las preguntas")
            .Produces(StatusCodes.Status200OK);
    }

    public static async Task<IResult> GetAllQuestionsAsync(
        [FromServices] IHttpContextAccessor contextAccessor,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var response = await sender.Send(GetAllQuestionsQuery.Instance, cancellationToken);

        return Results.Ok(response);
    }
}

public record GetAllQuestionsDto(Guid Id, string Name, Guid CreatedById, Guid? UpdatedById);

public record GetAllQuestionsQuery : IRequest<GetAllQuestionsDto[]>
{
    private static readonly Lazy<GetAllQuestionsQuery> Lazy = new();
    public static GetAllQuestionsQuery Instance => Lazy.Value;
}

public class GetAllQuestionsHandler : IRequestHandler<GetAllQuestionsQuery, GetAllQuestionsDto[]>
{
    private readonly IGetAllQuestionsRepository _repository;

    public GetAllQuestionsHandler(IGetAllQuestionsRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetAllQuestionsDto[]> Handle(GetAllQuestionsQuery request, CancellationToken cancellationToken)
    {
        return await _repository
            .GetAllQuestionsAsync(cancellationToken);
    }
}

public interface IGetAllQuestionsRepository
{
    Task<GetAllQuestionsDto[]> GetAllQuestionsAsync(CancellationToken cancellationToken = default);
}
using CrossCutting.EntityFramework;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VSlices.Core.DataAccess.Abstracts;
using VSlices.Core.Abstracts.Responses;
using VSlices.Core.Abstracts.Sender;
using VSlices.Core.Handlers;
using VSlices.Core.Presentation.AspNetCore;
using VSlices.Core.Abstracts.Requests;

// ReSharper disable once CheckNamespace
namespace Core.UseCases.ReadQuestion;

public class ReadQuestionSwaggerDocs : SwaggerDocumentation
{
    public override string Name => "ReadQuestion";
    public override string[] Tags => new[] { "Questions" };
    public override string Summary => "Obtiene todas las preguntas creadas";

    public override SwaggerResponse[] Responses => new[]
    {
        SwaggerResponse.WithJson.Of<ReadQuestionResponse[]>(StatusCodes.Status200OK,
            "Se obtuvieron las preguntas correctamente"),
    };
}

public class ReadQuestionEndpoint : AspNetCoreEndpointDefinition, IEndpointDefinition
{
    public override Delegate DelegateHandler => HandleAsync;
    public override string Route => "api/question";
    public override HttpMethod HttpMethod => HttpMethod.Get;
    public override SwaggerDocumentation? SwaggerConfiguration => new ReadQuestionSwaggerDocs();

    public static void DefineDependencies(IServiceCollection services)
    {
        services.AddScoped<IReadQuestionRepository, ReadQuestionRepository>();
    }

    public static async ValueTask<IResult> HandleAsync([FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.SendAsync(ReadQuestionQuery.Instance, cancellationToken);

        return result.MatchEndpointResult(TypedResults.Ok);
    }
}

public record ReadQuestionResponse(string Id, string Title, string Content);

public record ReadQuestionQuery : IQuery<ReadQuestionResponse[]>
{
    // Singleton of class
    public static readonly ReadQuestionQuery Instance = new();

}

public class ReadQuestionHandler : BasicReadHandler<ReadQuestionQuery, ReadQuestionResponse[]>
{
    public ReadQuestionHandler(IReadQuestionRepository repository) : base(repository) { }

    protected override ValueTask<Response<Success>> ValidateFeatureRulesAsync(
        ReadQuestionQuery request, CancellationToken cancellationToken)
        => ResponseDefaults.TaskSuccess;
}

public interface IReadQuestionRepository : IReadRepository<ReadQuestionResponse[]> { }

public class ReadQuestionRepository : IReadQuestionRepository
{
    private readonly AppDbContext _context;

    public ReadQuestionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Response<ReadQuestionResponse[]>> ReadAsync(CancellationToken cancellationToken = default)
        => await _context.Questions
            .Select(q => new ReadQuestionResponse(q.Id.ToString(), q.Title, q.Content))
            .ToArrayAsync(cancellationToken);
}

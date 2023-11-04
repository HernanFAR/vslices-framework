using CrossCutting.EntityFramework;
using Domain.Entities;
using Domain.Events;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net.Mime;
using VSlices.Core.Abstracts.Events;
using VSlices.Core.Abstracts.Requests;
using VSlices.Core.Abstracts.Responses;
using VSlices.Core.Abstracts.Sender;
using VSlices.Core.DataAccess.Abstracts;
using VSlices.Core.DataAccess.EntityFramework;
using VSlices.Core.Handlers.FluentValidation;
using VSlices.Core.Presentation.AspNetCore;

// ReSharper disable once CheckNamespace
namespace Core.UseCases.CreateQuestion;

public record CreateQuestionContract(string Title, string Content);

public class CreateQuestionSwaggerDocs : SwaggerDocumentation
{
    public override string Name => "CreateQuestion";
    public override string[] Tags => new[] { "Questions" };
    public override string Summary => "Crea una pregunta";
    public override string MainConsumingContentType => MediaTypeNames.Application.Json;

    public override SwaggerResponse[] Responses => new[]
    {
        SwaggerResponse.WithStatusCode(StatusCodes.Status200OK, "Se creo correctamente la pregunta"),
        SwaggerResponse.WithJson.OfProblemDetails(StatusCodes.Status422UnprocessableEntity, "Hubieron errores al crear la pregunta")
    };
}

public class CreateQuestionEndpoint : AspNetCoreEndpointDefinition, IEndpointDefinition
{
    public override Delegate DelegateHandler => HandleAsync;
    public override string Route => "api/question";
    public override HttpMethod HttpMethod => HttpMethod.Post;
    public override SwaggerDocumentation? SwaggerConfiguration => new CreateQuestionSwaggerDocs();

    public static void DefineDependencies(IServiceCollection services)
    {
        services.AddScoped<ICreateQuestionRepository, CreateQuestionRepository>();
    }

    public static async ValueTask<IResult> HandleAsync([FromServices] ISender sender,
        [FromBody] CreateQuestionContract contract, CancellationToken cancellationToken)
    {
        var command = new CreateQuestionCommand(contract.Title, contract.Content);
        var result = await sender.SendAsync(command, cancellationToken);

        return result.MatchEndpointResult(TypedResults.Ok);
    }
}

public class CreateQuestionValidator : AbstractValidator<CreateQuestionCommand>
{
    private readonly ICreateQuestionRepository _repository;

    public const string TitleEmptyMessage = "El título no puede estar vacío";
    public const string TitleContainsPutaMessage = "El título no puede contener la palabra puta";
    public const string TitleExistMessage = "El titulo ya existe en la base de datos";
    public static readonly string TitleMaxLengthMessage = $"El título no puede tener más de {Question.TitleField.TitleMaxLength} caracteres";

    public const string ContentEmptyMessage = "El contenido no puede estar vacío";
    public static readonly string ContentMaxLengthMessage = $"El contenido no puede tener más de {Question.ContentField.ContentMaxLength} caracteres";

    public CreateQuestionValidator(ICreateQuestionRepository repository)
    {
        _repository = repository;
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage(TitleEmptyMessage)
            .Must(x => !x.Contains("puta")).WithMessage(TitleContainsPutaMessage)
            .MustAsync(BeANotExistingTitle).WithMessage(TitleExistMessage)
            .MaximumLength(Question.TitleField.TitleMaxLength).WithMessage(TitleMaxLengthMessage);

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage(ContentEmptyMessage)
            .MaximumLength(Question.ContentField.ContentMaxLength)
            .WithMessage(ContentMaxLengthMessage);
    }

    private async Task<bool> BeANotExistingTitle(string title, CancellationToken cancellationToken)
        => await _repository.NotExistTitleAsync(title, cancellationToken);
}

public record CreateQuestionCommand(string Title, string Content) : ICommand;

public class CreateQuestionHandler : EntityFluentValidatedCreateHandler<CreateQuestionCommand, Question>
{
    private readonly IEventQueueWriter _eventWriter;

    public CreateQuestionHandler(IValidator<Question> entityValidator, ICreateQuestionRepository repository,
        IEventQueueWriter eventWriter)
        : base(entityValidator, repository)
    {
        _eventWriter = eventWriter;
    }

    protected override ValueTask<Response<Success>> ValidateFeatureRulesAsync(
        CreateQuestionCommand request, CancellationToken cancellationToken) => ResponseDefaults.TaskSuccess;

    protected override ValueTask<Question> CreateEntityAsync(
        CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid()
            .ToString()
            .Replace("-", "0");

        return ValueTask.FromResult(new Question(id, request.Title, request.Content));
    }

    protected override async ValueTask AfterCreationAsync(Question entity, CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        var @event = new QuestionModifiedEvent(ModificationType.Creation, entity.Id, entity.Title, entity.Content);

        await _eventWriter.EnqueueAsync(@event, cancellationToken);
    }
}

public interface ICreateQuestionRepository : ICreateRepository<Question>
{
    ValueTask<bool> NotExistTitleAsync(string title, CancellationToken cancellationToken);
}

public class CreateQuestionRepository : EFCreateRepository<AppDbContext, Question>, ICreateQuestionRepository
{
    private readonly AppDbContext _context;

    public CreateQuestionRepository(AppDbContext context, ILogger<CreateQuestionRepository> logger) : base(context, logger)
    {
        _context = context;
    }

    public async ValueTask<bool> NotExistTitleAsync(string title, CancellationToken cancellationToken)
    {
        return !(await _context.Questions.AnyAsync(x => x.Title == title, cancellationToken));
    }
}
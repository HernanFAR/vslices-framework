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
using VSlices.Core.DataAccess.Abstracts;
using VSlices.Core.Abstracts.Events;
using VSlices.Core.Abstracts.Responses;
using VSlices.Core.Abstracts.Sender;
using VSlices.Core.Handlers.FluentValidation;
using VSlices.Core.DataAccess.EntityFramework;
using VSlices.Core.Presentation.AspNetCore;
using VSlices.Core.Abstracts.Requests;

// ReSharper disable once CheckNamespace
namespace Core.UseCases.UpdateQuestion;

public record UpdateQuestionContract(string Title, string Content);

public class UpdateQuestionSwaggerDocs : SwaggerDocumentation
{
    public override string Name => "UpdateQuestion";
    public override string[] Tags => new[] { "Questions" };
    public override string Summary => "Actualiza una pregunta";
    public override string MainConsumingContentType => MediaTypeNames.Application.Json;
    public override SwaggerResponse[] Responses => new[]
    {
        SwaggerResponse.WithStatusCode(StatusCodes.Status200OK, "Se actualizo correctamente la pregunta"),
        SwaggerResponse.WithJson.OfProblemDetails(StatusCodes.Status404NotFound, "No se encontró la pregunta"),
        SwaggerResponse.WithJson.OfProblemDetails(StatusCodes.Status422UnprocessableEntity, "No se pudo actualizar la pregunta")
    };
}

public class UpdateQuestionEndpoint : AspNetCoreEndpointDefinition, IEndpointDefinition
{
    public override Delegate DelegateHandler => HandleAsync;
    public override string Route => "api/question/{id}";
    public override HttpMethod HttpMethod => HttpMethod.Put;
    public override SwaggerDocumentation? SwaggerConfiguration => new UpdateQuestionSwaggerDocs();

    public static void DefineDependencies(IServiceCollection services)
    {
        services.AddScoped<IUpdateQuestionRepository, UpdateQuestionRepository>();
    }

    public static async Task<IResult> HandleAsync([FromServices] ISender sender,
        [FromBody] UpdateQuestionContract contract, [FromQuery] string id,
        CancellationToken cancellationToken)
    {
        var command = new UpdateQuestionCommand(id, contract.Title, contract.Content);
        var result = await sender.SendAsync(command, cancellationToken);

        return result.MatchEndpointResult(TypedResults.Ok);
    }
}

public record UpdateQuestionCommand(string Id, string Title, string Content) : ICommand;

public class UpdateQuestionValidator : AbstractValidator<UpdateQuestionCommand>
{
    private readonly IUpdateQuestionRepository _repository;

    public const string TitleEmptyMessage = "El título no puede estar vacío";
    public const string TitleContainsPutaMessage = "El título no puede contener la palabra puta";
    public const string TitleExistMessage = "El titulo ya existe en la base de datos";
    public static readonly string TitleMaxLengthMessage = $"El título no puede tener más de {Question.TitleField.TitleMaxLength} caracteres";

    public const string ContentEmptyMessage = "El contenido no puede estar vacío";
    public static readonly string ContentMaxLengthMessage = $"El contenido no puede tener más de {Question.ContentField.ContentMaxLength} caracteres";

    public UpdateQuestionValidator(IUpdateQuestionRepository repository)
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

    private async Task<bool> BeANotExistingTitle(UpdateQuestionCommand command, string title, CancellationToken cancellationToken)
        => await _repository.NotExistTitleAsync(command.Id, title, cancellationToken);
}

public class UpdateQuestionHandler : EntityFluentValidatedUpdateHandler<UpdateQuestionCommand, Question>
{
    private readonly IUpdateQuestionRepository _repository;
    private readonly IEventQueueWriter _eventWriter;

    public UpdateQuestionHandler(IValidator<Question> entityValidator, IUpdateQuestionRepository repository,
        IEventQueueWriter eventWriter)
        : base(entityValidator, repository)
    {
        _repository = repository;
        _eventWriter = eventWriter;
    }

    protected override async ValueTask<Response<Success>> ValidateFeatureRulesAsync(
        UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        var exist = await _repository.ExistQuestionAsync(request.Id, cancellationToken);

        return exist
            ? Success.Value
            : BusinessFailure.Of.NotFoundResource();
    }

    protected override async ValueTask<Question> GetAndProcessEntityAsync(UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        var question = await _repository.GetQuestionAsync(request.Id, cancellationToken);

        question.UpdateState(request.Title, request.Content);

        return question;
    }

    protected override async ValueTask AfterUpdateAsync(Question entity, UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        var @event = new QuestionModifiedEvent(ModificationType.Modification, entity.Id, entity.Title, entity.Content);

        await _eventWriter.EnqueueAsync(@event, cancellationToken);
    }
}

public interface IUpdateQuestionRepository : IUpdateRepository<Question>
{
    ValueTask<bool> NotExistTitleAsync(string id, string title, CancellationToken cancellationToken);

    ValueTask<bool> ExistQuestionAsync(string id, CancellationToken cancellationToken);

    ValueTask<Question> GetQuestionAsync(string id, CancellationToken cancellationToken);
}

public class UpdateQuestionRepository : EFUpdateRepository<AppDbContext, Question>, IUpdateQuestionRepository
{
    private readonly AppDbContext _context;

    public UpdateQuestionRepository(AppDbContext context, ILogger<UpdateQuestionRepository> logger) : base(context, logger)
    {
        _context = context;
    }

    public async ValueTask<bool> NotExistTitleAsync(string id, string title, CancellationToken cancellationToken)
    {
        return !(await _context.Questions
            .Where(e => e.Id != id)
            .AnyAsync(x => x.Title == title, cancellationToken));
    }

    public async ValueTask<bool> ExistQuestionAsync(string id, CancellationToken cancellationToken)
    {
        return await _context.Questions
            .Where(e => e.Id == id)
            .AnyAsync(cancellationToken);
    }

    public async ValueTask<Question> GetQuestionAsync(string id, CancellationToken cancellationToken)
    {
        return await _context.Questions
            .Where(e => e.Id == id)
            .SingleAsync(cancellationToken);
    }
}
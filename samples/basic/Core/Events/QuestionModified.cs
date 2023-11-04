using Domain.Events;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using VSlices.Core.Abstracts.Handlers;
using VSlices.Core.Abstracts.Responses;

// ReSharper disable once CheckNamespace
namespace Core.Events.QuestionModified;

public class QuestionModifiedHandler : IHandler<QuestionModifiedEvent>
{
    private readonly ILogger<QuestionModifiedHandler> _logger;

    public QuestionModifiedHandler(ILogger<QuestionModifiedHandler> logger)
    {
        _logger = logger;
    }

    public ValueTask<Response<Success>> HandleAsync(QuestionModifiedEvent request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("QuestionModifiedEvent: {Question}", JsonSerializer.Serialize(request));

        return ResponseDefaults.TaskSuccess;
    }
}


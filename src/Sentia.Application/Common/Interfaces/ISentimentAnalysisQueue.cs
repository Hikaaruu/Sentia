using Sentia.Application.Features.Messages.Events;

namespace Sentia.Application.Common.Interfaces;

public interface ISentimentAnalysisQueue
{
    ValueTask QueueAsync(MessageCreatedEvent item);
    IAsyncEnumerable<MessageCreatedEvent> DequeueAsync(CancellationToken ct);
}

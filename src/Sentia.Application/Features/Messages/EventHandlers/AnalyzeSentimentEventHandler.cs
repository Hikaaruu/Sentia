using MediatR;
using Sentia.Application.Common.Interfaces;
using Sentia.Application.Features.Messages.Events;

namespace Sentia.Application.Features.Messages.EventHandlers;

public class AnalyzeSentimentEventHandler(ISentimentAnalysisQueue queue)
    : INotificationHandler<MessageCreatedEvent>
{
    public async Task Handle(MessageCreatedEvent notification, CancellationToken cancellationToken)
        => await queue.QueueAsync(notification);
}

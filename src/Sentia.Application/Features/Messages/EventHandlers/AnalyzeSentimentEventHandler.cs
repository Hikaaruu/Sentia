using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sentia.Application.Common.Interfaces;
using Sentia.Application.Features.Messages.Dtos;
using Sentia.Application.Features.Messages.Events;

namespace Sentia.Application.Features.Messages.EventHandlers;

public class AnalyzeSentimentEventHandler(
    IServiceScopeFactory scopeFactory,
    ILogger<AnalyzeSentimentEventHandler> logger)
    : INotificationHandler<MessageCreatedEvent>
{
    public Task Handle(MessageCreatedEvent notification, CancellationToken cancellationToken)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();

                var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
                var sentimentService = scope.ServiceProvider.GetRequiredService<ISentimentAnalysisService>();
                var signalRService = scope.ServiceProvider.GetRequiredService<ISignalRService>();

                var result = await sentimentService.AnalyzeAsync(notification.Content, CancellationToken.None);

                var message = await dbContext.Messages.FindAsync(
                    [notification.MessageId],
                    CancellationToken.None);

                if (message is not null)
                {
                    message.SentimentLabel = result.Label;
                    message.SentimentScore = result.Score;
                    await dbContext.SaveChangesAsync(CancellationToken.None);
                }

                var payload = new SentimentUpdatePayload(
                    MessageId: notification.MessageId,
                    ChatId: notification.ChatId,
                    SentimentLabel: result.Label,
                    SentimentScore: result.Score);

                await signalRService.BroadcastSentimentUpdateAsync(
                    notification.ParticipantUserIds,
                    payload,
                    CancellationToken.None);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Sentiment analysis failed for message {MessageId}", notification.MessageId);
            }
        }, cancellationToken);

        return Task.CompletedTask;
    }
}

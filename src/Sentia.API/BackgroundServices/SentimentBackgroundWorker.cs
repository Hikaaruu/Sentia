using MediatR;
using Sentia.Application.Common.Interfaces;
using Sentia.Application.Features.Messages.Commands.ProcessMessageSentiment;

namespace Sentia.API.BackgroundServices;

public sealed class SentimentBackgroundWorker(
    ISentimentAnalysisQueue queue,
    IServiceScopeFactory scopeFactory,
    ILogger<SentimentBackgroundWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var item in queue.DequeueAsync(stoppingToken))
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var sender = scope.ServiceProvider.GetRequiredService<ISender>();
                await sender.Send(
                    new ProcessMessageSentimentCommand(
                        item.MessageId,
                        item.ChatId,
                        item.Content,
                        item.ParticipantUserIds),
                    stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Sentiment analysis failed for message {MessageId}", item.MessageId);
            }
        }
    }
}

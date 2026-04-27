using MediatR;
using Sentia.Application.Common.Interfaces;
using Sentia.Application.Features.Messages.Dtos;

namespace Sentia.Application.Features.Messages.Commands.ProcessMessageSentiment;

internal sealed class ProcessMessageSentimentCommandHandler(
    IApplicationDbContext context,
    ISentimentAnalysisService sentimentService,
    ISignalRService signalRService)
    : IRequestHandler<ProcessMessageSentimentCommand>
{
    public async Task Handle(ProcessMessageSentimentCommand request, CancellationToken cancellationToken)
    {
        var result = await sentimentService.AnalyzeAsync(request.Content, cancellationToken);

        var message = await context.Messages.FindAsync([request.MessageId], cancellationToken);

        if (message is null)
            return;

        message.SentimentLabel = result.Label;
        message.SentimentScore = result.Score;
        await context.SaveChangesAsync(cancellationToken);

        var payload = new SentimentUpdatePayload(
            MessageId: request.MessageId,
            ChatId: request.ChatId,
            SentimentLabel: result.Label,
            SentimentScore: result.Score);

        await signalRService.BroadcastSentimentUpdateAsync(
            request.ParticipantUserIds,
            payload,
            cancellationToken);
    }
}

using Sentia.Application.Features.Messages.Dtos;

namespace Sentia.Application.Common.Interfaces;

public interface ISignalRService
{
    Task BroadcastNewMessageAsync(
        IEnumerable<string> userIds,
        NewMessagePayload payload,
        CancellationToken cancellationToken = default);

    Task BroadcastSentimentUpdateAsync(
        IEnumerable<string> userIds,
        SentimentUpdatePayload payload,
        CancellationToken cancellationToken = default);

    Task BroadcastReadReceiptAsync(
        string recipientUserId,
        ReadReceiptPayload payload,
        CancellationToken cancellationToken = default);

    Task BroadcastTypingAsync(
        IEnumerable<string> recipientUserIds,
        long chatId,
        string senderId,
        CancellationToken cancellationToken = default);
}

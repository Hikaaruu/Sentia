using MediatR;
using Sentia.Application.Common.Interfaces;
using Sentia.Application.Features.Messages.Dtos;
using Sentia.Application.Features.Messages.Events;

namespace Sentia.Application.Features.Messages.EventHandlers;

public class BroadcastMessageEventHandler(ISignalRService signalRService)
    : INotificationHandler<MessageCreatedEvent>
{
    public async Task Handle(MessageCreatedEvent notification, CancellationToken cancellationToken)
    {
        var payload = new NewMessagePayload(
            MessageId: notification.MessageId,
            ChatId: notification.ChatId,
            SenderId: notification.SenderId,
            Content: notification.Content,
            CreatedAt: notification.CreatedAt);

        await signalRService.BroadcastNewMessageAsync(
            notification.ParticipantUserIds,
            payload,
            cancellationToken);
    }
}

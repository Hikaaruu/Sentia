using MediatR;
using Microsoft.EntityFrameworkCore;
using Sentia.Application.Common.Interfaces;
using Sentia.Application.Features.Messages.Events;
using Sentia.Domain.Entities;

namespace Sentia.Application.Features.Messages.Commands.SendMessage;

public class SendMessageCommandHandler(
    IApplicationDbContext context,
    IPublisher publisher)
    : IRequestHandler<SendMessageCommand, SendMessageResult>
{
    public async Task<SendMessageResult> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var participantIds = await context.ChatParticipants
            .Where(cp => cp.ChatId == request.ChatId)
            .Select(cp => cp.UserId)
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;

        var message = new Message
        {
            Id = request.MessageId,
            ChatId = request.ChatId,
            SenderId = request.SenderId,
            Content = request.Content,
            CreatedAt = now,
            SentimentScore = null,
            SentimentLabel = null
        };

        context.Messages.Add(message);

        var chat = await context.Chats.FindAsync([request.ChatId], cancellationToken);
        if (chat is not null)
            chat.LastMessageAt = now;

        await context.SaveChangesAsync(cancellationToken);

        await publisher.Publish(new MessageCreatedEvent(
            MessageId: message.Id,
            ChatId: request.ChatId,
            SenderId: request.SenderId,
            Content: request.Content,
            CreatedAt: now,
            ParticipantUserIds: participantIds),
            cancellationToken);

        return new SendMessageResult(message.Id);
    }
}

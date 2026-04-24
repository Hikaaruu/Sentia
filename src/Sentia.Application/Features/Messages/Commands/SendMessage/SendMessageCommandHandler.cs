using MediatR;
using Microsoft.EntityFrameworkCore;
using Sentia.Application.Common.Exceptions;
using Sentia.Application.Common.Interfaces;
using Sentia.Application.Features.Messages.Events;
using Sentia.Domain.Entities;

namespace Sentia.Application.Features.Messages.Commands.SendMessage;

public class SendMessageCommandHandler(
    IApplicationDbContext context,
    IPublisher publisher)
    : IRequestHandler<SendMessageCommand, long>
{
    public async Task<long> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var participantIds = await context.ChatParticipants
            .Where(cp => cp.ChatId == request.ChatId)
            .Select(cp => cp.UserId)
            .ToListAsync(cancellationToken);

        if (!participantIds.Contains(request.SenderId))
            throw new ValidationException("ChatId", "You are not a participant of this chat.");

        var now = DateTime.UtcNow;

        var message = new Message
        {
            ChatId = request.ChatId,
            SenderId = request.SenderId,
            Content = request.Content,
            CreatedAt = now,
            SentimentScore = null,
            SentimentLabel = null
        };

        context.Messages.Add(message);

        var chat = await context.Chats.FindAsync([request.ChatId], cancellationToken);
        chat?.LastMessageAt = now;

        await context.SaveChangesAsync(cancellationToken);

        await publisher.Publish(new MessageCreatedEvent(
            MessageId: message.Id,
            ChatId: request.ChatId,
            SenderId: request.SenderId,
            Content: request.Content,
            CreatedAt: now,
            ParticipantUserIds: participantIds),
            cancellationToken);

        return message.Id;
    }
}

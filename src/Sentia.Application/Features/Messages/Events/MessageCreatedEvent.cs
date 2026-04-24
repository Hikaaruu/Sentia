using MediatR;

namespace Sentia.Application.Features.Messages.Events;

public record MessageCreatedEvent(
    long MessageId,
    long ChatId,
    string SenderId,
    string Content,
    DateTime CreatedAt,
    IEnumerable<string> ParticipantUserIds) : INotification;

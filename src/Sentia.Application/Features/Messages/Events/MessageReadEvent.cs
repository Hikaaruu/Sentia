using MediatR;

namespace Sentia.Application.Features.Messages.Events;

public record MessageReadEvent(
    string MessageId,
    long ChatId,
    string SenderId,
    string ReadByUserId) : INotification;

using MediatR;
using Sentia.Application.Common.Interfaces;

namespace Sentia.Application.Features.Messages.Commands.SendMessage;

public record SendMessageResult(string MessageId);

public record SendMessageCommand(
    string MessageId,
    long ChatId,
    string SenderId,
    string Content) : IRequest<SendMessageResult>, IRequireChatParticipantAuthorization
{
    string IAuthorize.RequestingUserId => SenderId;
}

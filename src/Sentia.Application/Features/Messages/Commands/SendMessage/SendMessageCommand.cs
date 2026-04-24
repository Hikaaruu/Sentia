using MediatR;

namespace Sentia.Application.Features.Messages.Commands.SendMessage;

public record SendMessageCommand(
    long ChatId,
    string SenderId,
    string Content) : IRequest<long>;

using MediatR;

namespace Sentia.Application.Features.Chats.Commands.CreateOrGetPrivateChat;

public record CreateOrGetPrivateChatResult(long ChatId, bool IsNew);

public record CreateOrGetPrivateChatCommand(
    string CurrentUserId,
    string RecipientUserId) : IRequest<CreateOrGetPrivateChatResult>;

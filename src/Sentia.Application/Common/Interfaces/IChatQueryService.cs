using Sentia.Application.Features.Chats.Dtos;

namespace Sentia.Application.Common.Interfaces;

public interface IChatQueryService
{
    Task<List<ChatSummaryDto>> GetUserChatsAsync(string userId, CancellationToken cancellationToken);
}
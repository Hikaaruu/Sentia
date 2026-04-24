using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentia.Application.Features.Chats.Commands.CreateOrGetPrivateChat;
using Sentia.Application.Features.Chats.Queries.GetUserChats;

namespace Sentia.API.Controllers;

[Authorize]
[ApiController]
[Route("api/chats")]
public class ChatsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateOrGetPrivateChat(
        [FromBody] CreateChatRequest request,
        CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await sender.Send(
            new CreateOrGetPrivateChatCommand(currentUserId, request.RecipientUserId),
            cancellationToken);

        return Ok(new { result.ChatId, result.IsNew });
    }

    [HttpGet]
    public async Task<IActionResult> GetUserChats(CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var chats = await sender.Send(new GetUserChatsQuery(currentUserId), cancellationToken);
        return Ok(chats);
    }
}

public record CreateChatRequest(string RecipientUserId);

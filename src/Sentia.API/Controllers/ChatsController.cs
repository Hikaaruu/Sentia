using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentia.Application.Features.Chats.Commands.CreateOrGetPrivateChat;
using Sentia.Application.Features.Chats.Commands.MarkChatAsRead;
using Sentia.Application.Features.Chats.Dtos;
using Sentia.Application.Features.Chats.Queries.GetUserChats;

namespace Sentia.API.Controllers;

[Authorize]
[ApiController]
[Route("api/chats")]
public class ChatsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CreateOrGetPrivateChatResult>> CreateOrGetPrivateChat(
        [FromBody] CreateChatRequest request,
        CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await sender.Send(
            new CreateOrGetPrivateChatCommand(currentUserId, request.RecipientUserId),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<ChatSummaryDto>>> GetUserChats(CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await sender.Send(new GetUserChatsQuery(currentUserId), cancellationToken);
        return Ok(result.Chats);
    }

    [HttpPost("{chatId}/read")]
    public async Task<IActionResult> MarkAsRead(
        long chatId,
        [FromBody] MarkAsReadRequest request,
        CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await sender.Send(
            new MarkChatAsReadCommand(chatId, currentUserId, request.MessageId),
            cancellationToken);

        return NoContent();
    }
}

public record CreateChatRequest(string RecipientUserId);
public record MarkAsReadRequest(string MessageId);

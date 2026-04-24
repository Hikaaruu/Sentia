using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentia.Application.Features.Messages.Commands.SendMessage;
using Sentia.Application.Features.Messages.Queries.GetChatMessages;

namespace Sentia.API.Controllers;

[Authorize]
[ApiController]
[Route("api/chats/{chatId}/messages")]
public class MessagesController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SendMessage(
        long chatId,
        [FromBody] SendMessageRequest request,
        CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var messageId = await sender.Send(
            new SendMessageCommand(chatId, currentUserId, request.Content),
            cancellationToken);

        return Ok(new { messageId });
    }

    [HttpGet]
    public async Task<IActionResult> GetMessages(
        long chatId,
        [FromQuery] long? before,
        [FromQuery] int take = 50,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var messages = await sender.Send(
            new GetChatMessagesQuery(chatId, currentUserId, before, take),
            cancellationToken);

        return Ok(messages);
    }
}

public record SendMessageRequest(string Content);

using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentia.Application.Features.Messages.Commands.SendMessage;
using Sentia.Application.Features.Messages.Dtos;
using Sentia.Application.Features.Messages.Queries.GetChatMessages;

namespace Sentia.API.Controllers;

[Authorize]
[ApiController]
[Route("api/chats/{chatId}/messages")]
public class MessagesController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<SendMessageResponse>> SendMessage(
        long chatId,
        [FromBody] SendMessageRequest request,
        CancellationToken cancellationToken)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await sender.Send(
            new SendMessageCommand(request.MessageId, chatId, currentUserId, request.Content),
            cancellationToken);

        return Ok(new SendMessageResponse(result.MessageId));
    }

    [HttpGet]
    public async Task<ActionResult<List<MessageDto>>> GetMessages(
        long chatId,
        [FromQuery] string? before,
        [FromQuery] int take = 50,
        CancellationToken cancellationToken = default)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await sender.Send(
            new GetChatMessagesQuery(chatId, currentUserId, before, take),
            cancellationToken);

        return Ok(result.Messages);
    }
}

public record SendMessageRequest(string MessageId, string Content);
public record SendMessageResponse(string MessageId);

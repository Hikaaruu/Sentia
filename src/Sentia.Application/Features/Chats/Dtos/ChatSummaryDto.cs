namespace Sentia.Application.Features.Chats.Dtos;

public record ChatSummaryDto(
    long ChatId,
    string OtherParticipantId,
    string OtherParticipantUsername,
    DateTime? LastMessageAt,
    string? LastMessageContent,
    string? LastMessageSenderId,
    int UnreadCount);

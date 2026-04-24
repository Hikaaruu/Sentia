namespace Sentia.Domain.Entities;

public class ChatReadStatus
{
    public required string UserId { get; set; }
    public long ChatId { get; set; }
    public string? LastReadMessageId { get; set; }
    public DateTime? ReadAt { get; set; }

    public Chat Chat { get; set; } = null!;
    public Message? LastReadMessage { get; set; }
}
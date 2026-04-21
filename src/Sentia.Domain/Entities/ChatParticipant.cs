namespace Sentia.Domain.Entities;

public class ChatParticipant
{
    public long ChatId { get; set; }
    public required string UserId { get; set; }
    public DateTime JoinedAt { get; set; }

    public Chat Chat { get; set; } = null!;
}
namespace Sentia.Domain.Entities;

public class Chat
{
    public long Id { get; set; }
    public ChatType Type { get; set; }
    public string? Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastMessageAt { get; set; }

    public ICollection<Message> Messages { get; set; } = [];
    public ICollection<ChatParticipant> Participants { get; set; } = [];
}

public enum ChatType : byte
{
    Private = 1,
    Group = 2
}
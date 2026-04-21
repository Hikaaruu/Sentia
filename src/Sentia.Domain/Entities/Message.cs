namespace Sentia.Domain.Entities;

public class Message
{
    public long Id { get; set; }
    public long ChatId { get; set; }
    public required string SenderId { get; set; }
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public double? SentimentScore { get; set; }
    public SentimentLabel? SentimentLabel { get; set; }

    public Chat Chat { get; set; } = null!;
}

public enum SentimentLabel
{
    Positive = 1,
    Neutral = 2,
    Negative = 3
}
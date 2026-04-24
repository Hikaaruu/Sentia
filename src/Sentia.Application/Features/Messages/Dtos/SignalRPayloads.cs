using Sentia.Domain.Entities;

namespace Sentia.Application.Features.Messages.Dtos;

public record NewMessagePayload(
    long MessageId,
    long ChatId,
    string SenderId,
    string Content,
    DateTime CreatedAt);

public record SentimentUpdatePayload(
    long MessageId,
    long ChatId,
    SentimentLabel SentimentLabel,
    double SentimentScore);

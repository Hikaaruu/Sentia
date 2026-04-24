using Sentia.Domain.Entities;

namespace Sentia.Application.Features.Messages.Dtos;

public record NewMessagePayload(
    string MessageId,
    long ChatId,
    string SenderId,
    string Content,
    DateTime CreatedAt);

public record SentimentUpdatePayload(
    string MessageId,
    long ChatId,
    SentimentLabel SentimentLabel,
    double SentimentScore);

public record ReadReceiptPayload(
    string MessageId,
    long ChatId,
    string ReadByUserId);

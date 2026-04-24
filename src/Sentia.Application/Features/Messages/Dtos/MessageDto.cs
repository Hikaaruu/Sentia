using Sentia.Domain.Entities;

namespace Sentia.Application.Features.Messages.Dtos;

public record MessageDto(
    long Id,
    long ChatId,
    string SenderId,
    string Content,
    DateTime CreatedAt,
    double? SentimentScore,
    SentimentLabel? SentimentLabel);

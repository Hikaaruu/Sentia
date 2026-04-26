using MediatR;

namespace Sentia.Application.Features.Messages.Commands.ProcessMessageSentiment;

public record ProcessMessageSentimentCommand(
    string MessageId,
    long ChatId,
    string Content,
    IEnumerable<string> ParticipantUserIds) : IRequest;

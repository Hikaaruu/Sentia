namespace Sentia.Application.Common.Interfaces;

/// <summary>
/// Marker interface for MediatR requests that require authorization.
/// </summary>
public interface IAuthorize
{
    string RequestingUserId { get; }
}

/// <summary>
/// Requires the requesting user to be a participant of the target chat.
/// </summary>
public interface IRequireChatParticipantAuthorization : IAuthorize
{
    long ChatId { get; }
}

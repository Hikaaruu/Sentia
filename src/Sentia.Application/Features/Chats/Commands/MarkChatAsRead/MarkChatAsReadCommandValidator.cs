using FluentValidation;

namespace Sentia.Application.Features.Chats.Commands.MarkChatAsRead;

public class MarkChatAsReadCommandValidator : AbstractValidator<MarkChatAsReadCommand>
{
    public MarkChatAsReadCommandValidator()
    {
        RuleFor(x => x.ChatId)
            .GreaterThan(0).WithMessage("ChatId must be a valid ID.");

        RuleFor(x => x.CurrentUserId)
            .NotEmpty().WithMessage("Current user ID is required.");

        RuleFor(x => x.MessageId)
            .NotEmpty().WithMessage("MessageId is required.")
            .MaximumLength(40).WithMessage("MessageId must not exceed 40 characters.");
    }
}

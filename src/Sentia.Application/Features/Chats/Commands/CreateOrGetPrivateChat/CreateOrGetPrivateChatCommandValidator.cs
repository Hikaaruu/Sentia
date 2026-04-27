using FluentValidation;

namespace Sentia.Application.Features.Chats.Commands.CreateOrGetPrivateChat;

public class CreateOrGetPrivateChatCommandValidator : AbstractValidator<CreateOrGetPrivateChatCommand>
{
    public CreateOrGetPrivateChatCommandValidator()
    {
        RuleFor(x => x.CurrentUserId)
            .NotEmpty().WithMessage("Current user ID is required.")
            .MaximumLength(450).WithMessage("Current user ID must not exceed 450 characters.");

        RuleFor(x => x.RecipientUserId)
            .NotEmpty().WithMessage("Recipient user ID is required.")
            .MaximumLength(450).WithMessage("Recipient user ID must not exceed 450 characters.")
            .NotEqual(x => x.CurrentUserId).WithMessage("Cannot start a chat with yourself.");
    }
}

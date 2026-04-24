using FluentValidation;

namespace Sentia.Application.Features.Chats.Commands.CreateOrGetPrivateChat;

public class CreateOrGetPrivateChatCommandValidator : AbstractValidator<CreateOrGetPrivateChatCommand>
{
    public CreateOrGetPrivateChatCommandValidator()
    {
        RuleFor(x => x.CurrentUserId)
            .NotEmpty().WithMessage("Current user ID is required.");

        RuleFor(x => x.RecipientUserId)
            .NotEmpty().WithMessage("Recipient user ID is required.")
            .NotEqual(x => x.CurrentUserId).WithMessage("Cannot start a chat with yourself.");
    }
}

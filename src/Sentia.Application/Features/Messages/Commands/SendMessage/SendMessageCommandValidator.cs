using FluentValidation;

namespace Sentia.Application.Features.Messages.Commands.SendMessage;

public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        RuleFor(x => x.MessageId)
            .NotEmpty().WithMessage("MessageId is required.")
            .MaximumLength(40).WithMessage("MessageId must not exceed 40 characters.");

        RuleFor(x => x.ChatId)
            .GreaterThan(0).WithMessage("ChatId must be a valid ID.");

        RuleFor(x => x.SenderId)
            .NotEmpty().WithMessage("SenderId is required.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Message content cannot be empty.")
            .MaximumLength(4000).WithMessage("Message content must not exceed 4000 characters.");
    }
}

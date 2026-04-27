using FluentValidation;

namespace Sentia.Application.Features.Messages.Queries.GetChatMessages;

public class GetChatMessagesQueryValidator : AbstractValidator<GetChatMessagesQuery>
{
    public GetChatMessagesQueryValidator()
    {
        RuleFor(x => x.ChatId)
            .GreaterThan(0).WithMessage("ChatId must be a valid ID.");

        RuleFor(x => x.CurrentUserId)
            .NotEmpty().WithMessage("Current user ID is required.");

        RuleFor(x => x.Before)
            .MaximumLength(40).WithMessage("Before cursor must not exceed 40 characters.")
            .When(x => x.Before is not null);

        RuleFor(x => x.Take)
            .InclusiveBetween(1, 100).WithMessage("Take must be between 1 and 100.");
    }
}

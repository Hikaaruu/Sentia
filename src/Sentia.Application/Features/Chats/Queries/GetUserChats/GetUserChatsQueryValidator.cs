using FluentValidation;

namespace Sentia.Application.Features.Chats.Queries.GetUserChats;

public class GetUserChatsQueryValidator : AbstractValidator<GetUserChatsQuery>
{
    public GetUserChatsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}

using FluentValidation;

namespace Social.Application.Features.Friendships.Commands.Update.AcceptFriendship;

public class AcceptFriendshipCommandValidator : AbstractValidator<AcceptFriendshipCommand>
{
    public AcceptFriendshipCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage($"The value cannot be empty: {nameof(AcceptFriendshipCommand.TenantId)} ");

        RuleFor(x => x.FriendTag)
            .NotNull().WithMessage($"Value '{nameof(AcceptFriendshipCommand.FriendTag)}' is required.")
            .NotEmpty().WithMessage($"The value cannot be empty: {nameof(AcceptFriendshipCommand.FriendTag)} ");
    }
}

using FluentValidation;

namespace Social.Application.Features.Friendships.Commands.Create;

public class CreateFriendshipCommandValidator : AbstractValidator<CreateFriendshipCommand>
{
    public CreateFriendshipCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage($"The value cannot be empty: {nameof(CreateFriendshipCommand.TenantId)} ");

        RuleFor(x => x.FriendTag)
            .NotNull().WithMessage($"Value '{nameof(CreateFriendshipCommand.FriendTag)}' is required.")
            .NotEmpty().WithMessage($"The value cannot be empty: {nameof(CreateFriendshipCommand.FriendTag)} ");
    }
}

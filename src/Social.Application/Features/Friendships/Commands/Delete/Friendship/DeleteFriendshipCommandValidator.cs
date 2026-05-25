using FluentValidation;

namespace Social.Application.Features.Friendships.Commands.Delete.Friendship;

public class DeleteFriendshipCommandValidator : AbstractValidator<DeleteFriendshipCommand>
{
    public DeleteFriendshipCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(x => x.TenantId)
            .NotNull().WithMessage($"Value '{nameof(DeleteFriendshipCommand.TenantId)}' is required.")
            .NotEmpty().WithMessage($"The value cannot be empty: {nameof(DeleteFriendshipCommand.TenantId)} ");

        RuleFor(x => x.FriendTag)
            .NotNull().WithMessage($"Value '{nameof(DeleteFriendshipCommand.FriendTag)}' is required.")
            .NotEmpty().WithMessage($"The value cannot be empty: {nameof(DeleteFriendshipCommand.FriendTag)} ")
            .Must(x => x.Length <= 20)
            .WithMessage($"Value '{nameof(DeleteFriendshipCommand.FriendTag)}' should not exceed 20.");
    }
}

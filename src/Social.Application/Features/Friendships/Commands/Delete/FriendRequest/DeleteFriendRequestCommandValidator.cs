using FluentValidation;

namespace Social.Application.Features.Friendships.Commands.Delete.FriendRequest;

public sealed class DeleteFriendRequestCommandValidator : AbstractValidator<DeleteFriendRequestCommand>
{
    public DeleteFriendRequestCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage($"The value cannot be empty: {nameof(DeleteFriendRequestCommand.TenantId)} ");

        RuleFor(x => x.FriendTag)
            .NotEmpty()
            .WithMessage($"The value cannot be empty: {nameof(DeleteFriendRequestCommand.FriendTag)} ");
    }
}

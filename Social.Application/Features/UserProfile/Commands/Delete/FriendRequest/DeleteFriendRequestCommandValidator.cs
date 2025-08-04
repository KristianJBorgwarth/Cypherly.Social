using Social.Domain.Common;
using FluentValidation;

namespace Social.Application.Features.UserProfile.Commands.Delete.FriendRequest;

public sealed class DeleteFriendRequestCommandValidator : AbstractValidator<DeleteFriendRequestCommand>
{
    public DeleteFriendRequestCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage(Errors.General.ValueIsEmpty(nameof(DeleteFriendRequestCommand.TenantId)).Message);

        RuleFor(x => x.FriendTag)
            .NotEmpty()
            .WithMessage(Errors.General.ValueIsEmpty(nameof(DeleteFriendRequestCommand.FriendTag)).Message);
    }
}
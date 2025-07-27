using Cypherly.UserManagement.Domain.Common;
using FluentValidation;

namespace Social.Application.Features.UserProfile.Commands.Delete.FriendRequest;

public sealed class DeleteFriendRequestCommandValidator : AbstractValidator<DeleteFriendRequestCommand>
{
    public DeleteFriendRequestCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(Errors.General.ValueIsEmpty(nameof(DeleteFriendRequestCommand.Id)).Message);

        RuleFor(x => x.FriendTag)
            .NotEmpty()
            .WithMessage(Errors.General.ValueIsEmpty(nameof(DeleteFriendRequestCommand.FriendTag)).Message);
    }
}
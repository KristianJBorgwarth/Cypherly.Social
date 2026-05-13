using FluentValidation;
using Social.Domain.Common;

namespace Social.Application.Features.Friendships.Commands.Delete.Friendship;

public class DeleteFriendshipCommandValidator : AbstractValidator<DeleteFriendshipCommand>
{
    public DeleteFriendshipCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(x => x.TenantId)
            .NotNull().WithMessage(Errors.General.ValueIsRequired(nameof(DeleteFriendshipCommand.TenantId)).Message)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(DeleteFriendshipCommand.TenantId)).Message);

        RuleFor(x => x.FriendTag)
            .NotNull().WithMessage(Errors.General.ValueIsRequired(nameof(DeleteFriendshipCommand.FriendTag)).Message)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(DeleteFriendshipCommand.FriendTag)).Message)
            .Must(x => x.Length <= 20)
            .WithMessage(Errors.General.ValueTooLarge(nameof(DeleteFriendshipCommand.FriendTag), 20).Message);

    }
}
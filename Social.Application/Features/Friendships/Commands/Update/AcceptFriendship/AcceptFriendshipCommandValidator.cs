using FluentValidation;
using Social.Domain.Common;

namespace Social.Application.Features.Friendships.Commands.Update.AcceptFriendship;

public class AcceptFriendshipCommandValidator : AbstractValidator<AcceptFriendshipCommand>
{
    public AcceptFriendshipCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(AcceptFriendshipCommand.TenantId)).Message);

        RuleFor(x => x.FriendTag)
            .NotNull().WithMessage(Errors.General.ValueIsRequired(nameof(AcceptFriendshipCommand.FriendTag)).Message)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(AcceptFriendshipCommand.FriendTag)).Message);
    }
}
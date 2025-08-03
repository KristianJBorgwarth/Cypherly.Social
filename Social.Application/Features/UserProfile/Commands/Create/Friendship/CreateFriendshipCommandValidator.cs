using Social.Domain.Common;
using FluentValidation;

namespace Social.Application.Features.UserProfile.Commands.Create.Friendship;

public class CreateFriendshipCommandValidator : AbstractValidator<CreateFriendshipCommand>
{
    public CreateFriendshipCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(CreateFriendshipCommand.TenantId)).Message);

        RuleFor(x => x.FriendTag)
            .NotNull().WithMessage(Errors.General.ValueIsRequired(nameof(CreateFriendshipCommand.FriendTag)).Message)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(CreateFriendshipCommand.FriendTag)).Message);
    }
}
using FluentValidation;
using Social.Domain.Common;

namespace Social.Application.Features.Friendships.Commands.Create;

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
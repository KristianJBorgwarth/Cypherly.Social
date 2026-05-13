using FluentValidation;
using Social.Domain.Common;

namespace Social.Application.Features.Friendships.Commands.Update.BlockUser;

public class BlockUserCommandValidator : AbstractValidator<BlockUserCommand>
{
    public BlockUserCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage(Errors.General.ValueIsRequired(nameof(BlockUserCommand.TenantId)).Message);

        RuleFor(x => x.BlockedUserTag)
            .NotEmpty().WithMessage(Errors.General.ValueIsRequired(nameof(BlockUserCommand)).Message);
    }
}
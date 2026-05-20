using FluentValidation;

namespace Social.Application.Features.Friendships.Commands.Update.BlockUser;

public class BlockUserCommandValidator : AbstractValidator<BlockUserCommand>
{
    public BlockUserCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage($"Value '{nameof(BlockUserCommand.TenantId)}' is required.");

        RuleFor(x => x.BlockedUserTag)
            .NotEmpty().WithMessage($"Value '{nameof(BlockUserCommand)}' is required.");
    }
}

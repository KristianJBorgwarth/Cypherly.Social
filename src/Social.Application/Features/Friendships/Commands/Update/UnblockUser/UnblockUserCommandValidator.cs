using FluentValidation;

namespace Social.Application.Features.Friendships.Commands.Update.UnblockUser;

public class UnblockUserCommandValidator : AbstractValidator<UnblockUserCommand>
{
    public UnblockUserCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage($"The value cannot be empty: {nameof(UnblockUserCommand.TenantId)} ");
        RuleFor(x => x.Tag)
            .NotNull().WithMessage($"Value '{nameof(UnblockUserCommand.Tag)}' is required.")
            .NotEmpty().WithMessage($"The value cannot be empty: {nameof(UnblockUserCommand.Tag)} ");
    }
}

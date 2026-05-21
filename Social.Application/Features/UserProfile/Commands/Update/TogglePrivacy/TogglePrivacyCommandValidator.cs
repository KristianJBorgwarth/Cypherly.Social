using FluentValidation;

namespace Social.Application.Features.UserProfile.Commands.Update.TogglePrivacy;

public class TogglePrivacyCommandValidator : AbstractValidator<TogglePrivacyCommand>
{
    public TogglePrivacyCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage($"The value cannot be empty: {nameof(TogglePrivacyCommand.TenantId)} ");
    }
}

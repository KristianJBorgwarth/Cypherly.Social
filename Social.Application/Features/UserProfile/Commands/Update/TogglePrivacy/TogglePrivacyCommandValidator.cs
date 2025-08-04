using Social.Domain.Common;
using FluentValidation;

namespace Social.Application.Features.UserProfile.Commands.Update.TogglePrivacy;

public class TogglePrivacyCommandValidator : AbstractValidator<TogglePrivacyCommand>
{
    public TogglePrivacyCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(TogglePrivacyCommand.TenantId)).Message);
    }
}
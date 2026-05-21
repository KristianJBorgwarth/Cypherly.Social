using FluentValidation;

namespace Social.Application.Features.UserProfile.Commands.Update.DisplayName;

public class UpdateUserProfileDisplayNameCommandValidator : AbstractValidator<UpdateUserProfileDisplayNameCommand>
{
    public UpdateUserProfileDisplayNameCommandValidator()
    {
        RuleFor(cmd => cmd.TenantId)
            .NotNull().NotEmpty().WithMessage($"The value cannot be empty: {nameof(UpdateUserProfileDisplayNameCommand.TenantId)} ");
        RuleFor(cmd => cmd.DisplayName)
            .NotNull().WithMessage($"Value '{nameof(UpdateUserProfileDisplayNameCommand.DisplayName)}' is required.")
            .NotEmpty().WithMessage($"The value cannot be empty: {nameof(UpdateUserProfileDisplayNameCommand.DisplayName)} ");
    }
}

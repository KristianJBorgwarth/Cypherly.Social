
using FluentValidation;

namespace Social.Application.Features.UserProfile.Commands.Update.Avatar;

public class UpdateAvatarCommandValidator : AbstractValidator<UpdateAvatarCommand>
{
    public UpdateAvatarCommandValidator()
    {
        RuleFor(cmd => cmd.TenantId)
            .NotNull().WithMessage($"Value '{nameof(UpdateAvatarCommand.TenantId)}' is required.")
            .NotEmpty()
            .WithMessage($"The value cannot be empty: {nameof(UpdateAvatarCommand.TenantId)} ");

        RuleFor(cmd => cmd.NewProfilePicture)
            .NotNull().WithMessage($"Value '{nameof(UpdateAvatarCommand.NewProfilePicture)}' is required.")
            .NotEmpty().WithMessage($"The value cannot be empty: {nameof(UpdateAvatarCommand.NewProfilePicture)} ");
    }
}

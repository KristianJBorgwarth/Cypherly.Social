using FluentValidation;

namespace Social.Application.Features.UserProfile.Commands.Update.ProfilePicture;

public class UpdateUserProfilePictureCommandValidator : AbstractValidator<UpdateUserProfilePictureCommand>
{
    public UpdateUserProfilePictureCommandValidator()
    {
        RuleFor(cmd => cmd.TenantId)
            .NotNull().WithMessage($"Value '{nameof(UpdateUserProfilePictureCommand.TenantId)}' is required.")
            .NotEmpty()
            .WithMessage($"The value cannot be empty: {nameof(UpdateUserProfilePictureCommand.TenantId)} ");

        RuleFor(cmd => cmd.NewProfilePicture)
            .NotNull().WithMessage($"Value '{nameof(UpdateUserProfilePictureCommand.NewProfilePicture)}' is required.")
            .NotEmpty().WithMessage($"The value cannot be empty: {nameof(UpdateUserProfilePictureCommand.NewProfilePicture)} ");
    }
}

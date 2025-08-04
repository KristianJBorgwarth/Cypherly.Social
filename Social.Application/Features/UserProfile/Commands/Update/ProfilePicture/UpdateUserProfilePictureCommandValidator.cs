using Social.Domain.Common;
using FluentValidation;

namespace Social.Application.Features.UserProfile.Commands.Update.ProfilePicture;

public class UpdateUserProfilePictureCommandValidator : AbstractValidator<UpdateUserProfilePictureCommand>
{
    public UpdateUserProfilePictureCommandValidator()
    {
        RuleFor(cmd => cmd.TenantId)
            .NotNull().WithMessage(Errors.General.ValueIsRequired(nameof(UpdateUserProfilePictureCommand.TenantId))
                .Message)
            .NotEmpty()
            .WithMessage(Errors.General.ValueIsEmpty(nameof(UpdateUserProfilePictureCommand.TenantId)).Message);

        RuleFor(cmd => cmd.NewProfilePicture)
            .NotNull().WithMessage(Errors.General
                .ValueIsRequired(nameof(UpdateUserProfilePictureCommand.NewProfilePicture)).Message)
            .NotEmpty().WithMessage(Errors.General
                .ValueIsEmpty(nameof(UpdateUserProfilePictureCommand.NewProfilePicture)).Message);
    }
}
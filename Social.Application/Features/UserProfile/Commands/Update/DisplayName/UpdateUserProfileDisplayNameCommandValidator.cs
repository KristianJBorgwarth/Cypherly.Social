using Cypherly.UserManagement.Domain.Common;
using FluentValidation;

namespace Social.Application.Features.UserProfile.Commands.Update.DisplayName;

public class UpdateUserProfileDisplayNameCommandValidator : AbstractValidator<UpdateUserProfileDisplayNameCommand>
{
    public UpdateUserProfileDisplayNameCommandValidator()
    {
        RuleFor(cmd => cmd.Id)
            .NotNull().NotEmpty().WithMessage(Errors.General
                .ValueIsEmpty(nameof(UpdateUserProfileDisplayNameCommand.Id)).Message);
        RuleFor(cmd => cmd.DisplayName)
            .NotNull().WithMessage(Errors.General.ValueIsRequired(nameof(UpdateUserProfileDisplayNameCommand.DisplayName)).Message)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(UpdateUserProfileDisplayNameCommand.DisplayName)).Message);
    }
}
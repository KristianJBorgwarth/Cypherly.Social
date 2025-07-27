using Cypherly.UserManagement.Domain.Common;
using FluentValidation;

namespace Social.Application.Features.UserProfile.Commands.Update.UnblockUser;

public class UnblockUserCommandValidator : AbstractValidator<UnblockUserCommand>
{
    public UnblockUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(UnblockUserCommand.Id)).Message);
        RuleFor(x => x.Tag)
            .NotNull().WithMessage(Errors.General.ValueIsRequired(nameof(UnblockUserCommand.Tag)).Message)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(UnblockUserCommand.Tag)).Message);
    }
}
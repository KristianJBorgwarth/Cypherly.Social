using Social.Domain.Common;
using FluentValidation;

namespace Social.Application.Features.UserProfile.Commands.Update.UnblockUser;

public class UnblockUserCommandValidator : AbstractValidator<UnblockUserCommand>
{
    public UnblockUserCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(UnblockUserCommand.TenantId)).Message);
        RuleFor(x => x.Tag)
            .NotNull().WithMessage(Errors.General.ValueIsRequired(nameof(UnblockUserCommand.Tag)).Message)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(UnblockUserCommand.Tag)).Message);
    }
}
using Cypherly.UserManagement.Domain.Common;
using FluentValidation;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.MarkFriendRequestAsSeen;

public class MarkFriendRequestsReadCommandValidator : AbstractValidator<MarkFriendRequestsReadCommand>
{
    public MarkFriendRequestsReadCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(Errors.General.ValueIsEmpty(nameof(MarkFriendRequestsReadCommand.Id)).Message);

        RuleFor(x => x.RequestTags)
            .NotEmpty()
            .WithMessage(Errors.General.ValueIsEmpty(nameof(MarkFriendRequestsReadCommand.RequestTags)).Message)
            .Must(tags => tags.All(tag => !string.IsNullOrWhiteSpace(tag)))
            .WithMessage(Errors.General.ValueIsEmpty(nameof(MarkFriendRequestsReadCommand.RequestTags)).Message);
    }
}
using FluentValidation;
using Social.Domain.Common;

namespace Social.Application.Features.Friendships.Commands.Update.MarkFriendRequestAsSeen;

public class MarkFriendRequestsAsSeenCommandValidator : AbstractValidator<MarkFriendRequestsAsSeenCommand>
{
    public MarkFriendRequestsAsSeenCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage(Errors.General.ValueIsEmpty(nameof(MarkFriendRequestsAsSeenCommand.TenantId)).Message);

        RuleFor(x => x.RequestTags)
            .NotEmpty()
            .WithMessage(Errors.General.ValueIsEmpty(nameof(MarkFriendRequestsAsSeenCommand.RequestTags)).Message)
            .Must(tags => tags.All(tag => !string.IsNullOrWhiteSpace(tag)))
            .WithMessage(Errors.General.ValueIsEmpty(nameof(MarkFriendRequestsAsSeenCommand.RequestTags)).Message);
    }
}
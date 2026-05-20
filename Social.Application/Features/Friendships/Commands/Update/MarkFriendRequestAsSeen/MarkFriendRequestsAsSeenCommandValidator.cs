using FluentValidation;

namespace Social.Application.Features.Friendships.Commands.Update.MarkFriendRequestAsSeen;

public class MarkFriendRequestsAsSeenCommandValidator : AbstractValidator<MarkFriendRequestsAsSeenCommand>
{
    public MarkFriendRequestsAsSeenCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage($"The value cannot be empty: {nameof(MarkFriendRequestsAsSeenCommand.TenantId)} ");

        RuleFor(x => x.RequestTags)
            .NotEmpty()
            .WithMessage($"The value cannot be empty: {nameof(MarkFriendRequestsAsSeenCommand.RequestTags)} ")
            .Must(tags => tags.All(tag => !string.IsNullOrWhiteSpace(tag)))
            .WithMessage($"The value cannot be empty: {nameof(MarkFriendRequestsAsSeenCommand.RequestTags)} ");
    }
}

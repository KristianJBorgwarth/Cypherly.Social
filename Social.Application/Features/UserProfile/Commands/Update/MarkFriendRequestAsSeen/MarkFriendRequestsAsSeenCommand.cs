using Social.Application.Abstractions;

namespace Social.Application.Features.UserProfile.Commands.Update.MarkFriendRequestAsSeen;

public class MarkFriendRequestsAsSeenCommand : ICommand
{
    public required Guid TenantId { get; init; }
    public required IReadOnlyCollection<string> RequestTags { get; init; }
}
using Social.Application.Abstractions;

namespace Social.Application.Features.UserProfile.Commands.Update.MarkFriendRequestAsSeen;

public class MarkFriendRequestsReadCommand : ICommandId
{
    public required Guid Id { get; init; }
    public required IReadOnlyCollection<string> RequestTags { get; init; }
}
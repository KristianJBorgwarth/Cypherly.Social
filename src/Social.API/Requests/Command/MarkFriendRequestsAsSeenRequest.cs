namespace Social.API.Requests.Command;

public sealed record MarkFriendRequestsAsSeenRequest
{
    public required IReadOnlyCollection<string> RequestTags { get; init; }
}
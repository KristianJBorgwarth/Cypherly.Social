namespace Social.API.Requests.Command;

public sealed record DeleteFriendshipRequest
{
    public required string FriendTag { get; init; }
}
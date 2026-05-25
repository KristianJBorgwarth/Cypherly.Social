namespace Social.API.Requests.Command;

public sealed record DeleteFriendRequest
{
    public required string FriendTag { get; init; }
}
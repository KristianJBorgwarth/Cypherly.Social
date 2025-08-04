namespace Social.API.Requests.Command;

public sealed record CreateFriendshipRequest
{
    public required string FriendTag { get; init; }
}
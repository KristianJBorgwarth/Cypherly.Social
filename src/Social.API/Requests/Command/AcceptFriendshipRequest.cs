namespace Social.API.Requests.Command;

public sealed class AcceptFriendshipRequest
{
    public required string FriendTag { get; init; }
}
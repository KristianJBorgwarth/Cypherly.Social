namespace Social.API.Requests.Command;

public sealed record BlockUserRequest
{
    public required string BlockedUserTag { get; init; }
}
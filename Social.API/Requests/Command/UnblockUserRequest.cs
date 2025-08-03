namespace Social.API.Requests.Command;

public sealed record UnblockUserRequest
{
    public required string Tag { get; init; }
}
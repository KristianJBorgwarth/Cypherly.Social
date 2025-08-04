namespace Social.API.Requests.Query;

public sealed record GetUserProfileRequest
{
    public required Guid ExclusiveConnectionId { get; init; }
}
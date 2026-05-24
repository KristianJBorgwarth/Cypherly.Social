namespace Social.API.Requests.Query;

public sealed record GetAvatarRequest
{
    public required Guid AvatarId { get; init; }
}

namespace Social.API.Requests;

public sealed record GetUserProfileByTagRequest
{
    public required Guid TenantId { get; init; }
    public required string Tag { get; init; }
}
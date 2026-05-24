
namespace Social.Application.Features.UserProfile.Queries.GetAvatar;

public sealed record GetAvatarDto
{
    public Guid AvatarId { get; init; }
    public Stream? Content { get; init; }
    public string? ContentType { get; init; }
    public bool IsModified { get; init; }
    public string? ETag { get; init; }
}

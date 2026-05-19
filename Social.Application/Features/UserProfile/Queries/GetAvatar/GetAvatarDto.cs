using Social.Domain.ValueObjects;

namespace Social.Application.Features.UserProfile.Queries.GetAvatar;

public sealed record GetAvatarDto
{
    public Guid AvatarId { get; init; }
    public StreamContent? Content { get; init; }
    public string? ContentType { get; init; }
    public ETag? ETag { get; init; }
}

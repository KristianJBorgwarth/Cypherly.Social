using Social.Application.Abstractions;

namespace Social.Application.Features.UserProfile.Queries.GetAvatar;

public sealed record GetAvatarQuery : IQuery<GetAvatarDto>
{
    public required Guid AvatarId { get; init; }
    public string? ETag { get; init; }
}

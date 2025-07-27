using Social.Domain.Enums;

namespace Social.Domain.Dtos;

public sealed record FriendshipStatusDto
{
    public FriendshipStatus? Status { get; init; }
    public FriendshipDirection Direction { get; init; }
}
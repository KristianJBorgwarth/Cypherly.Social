using Cypherly.UserManagement.Domain.Enums;

namespace Cypherly.UserManagement.Domain.Dtos;

public sealed record FriendshipStatusDto
{
    public FriendshipStatus? Status { get; init; }
    public FriendshipDirection Direction { get; init; }
}
using Social.Application.Abstractions;

namespace Social.Application.Features.UserProfile.Commands.Delete.Friendship;

public sealed record DeleteFriendshipCommand : ICommand
{
    public required Guid TenantId { get; init; }
    public required string FriendTag { get; init; }
}
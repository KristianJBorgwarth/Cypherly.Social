using Social.Application.Abstractions;

namespace Social.Application.Features.Friendships.Commands.Delete.FriendRequest;

public sealed record DeleteFriendRequestCommand : ICommand
{
    public required Guid TenantId { get; init; }
    public required string FriendTag { get; init; }
}
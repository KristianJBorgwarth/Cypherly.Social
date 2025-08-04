using Social.Application.Abstractions;

namespace Social.Application.Features.UserProfile.Commands.Create.Friendship;

public sealed record CreateFriendshipCommand : ICommand
{
    public Guid TenantId { get; init; }
    public required string FriendTag { get; init; }
}
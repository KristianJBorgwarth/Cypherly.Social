using Cypherly.Application.Abstractions;
using Social.Application.Abstractions;

namespace Social.Application.Features.UserProfile.Commands.Delete.Friendship;

public sealed record DeleteFriendshipCommand : ICommandId
{
    public required Guid Id { get; init; }
    public required string FriendTag { get; init; }
}
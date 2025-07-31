using Social.Application.Abstractions;

namespace Social.Application.Features.UserProfile.Commands.Create.Friendship;

public sealed record CreateFriendshipCommand : ICommand
{
    public Guid Id { get; init; }
    public required string FriendTag { get; init; }
}
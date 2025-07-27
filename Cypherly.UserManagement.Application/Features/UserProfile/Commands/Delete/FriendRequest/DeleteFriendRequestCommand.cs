using Cypherly.Application.Abstractions;
using Cypherly.UserManagement.Application.Abstractions;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Delete.FriendRequest;

public sealed record DeleteFriendRequestCommand : ICommandId
{
    public required Guid Id { get; init; }
    public required string FriendTag { get; init; }
}
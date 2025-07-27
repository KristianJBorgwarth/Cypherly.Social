using Cypherly.Application.Abstractions;
using Cypherly.UserManagement.Application.Abstractions;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.AcceptFriendship;

public sealed record AcceptFriendshipCommand : ICommandId<AcceptFriendshipDto>
{
    public required Guid Id { get; init; }
    public required string FriendTag { get; init; }
}
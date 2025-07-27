using Cypherly.Application.Abstractions;
using Social.Application.Abstractions;

namespace Social.Application.Features.UserProfile.Commands.Update.AcceptFriendship;

public sealed record AcceptFriendshipCommand : ICommandId<AcceptFriendshipDto>
{
    public required Guid Id { get; init; }
    public required string FriendTag { get; init; }
}
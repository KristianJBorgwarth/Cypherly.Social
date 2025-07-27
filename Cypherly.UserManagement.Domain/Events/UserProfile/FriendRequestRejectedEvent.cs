using Cypherly.UserManagement.Domain.Abstractions;

namespace Cypherly.UserManagement.Domain.Events.UserProfile;

public sealed record FriendRequestRejectedEvent(Guid UserId, Guid DeletedFriendId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
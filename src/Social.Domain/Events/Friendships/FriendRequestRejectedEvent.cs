using Social.Domain.Abstractions;

namespace Social.Domain.Events.Friendships;

public sealed record FriendRequestRejectedEvent(Guid UserId, Guid DeletedFriendId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
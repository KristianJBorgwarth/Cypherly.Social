using Social.Domain.Abstractions;

namespace Social.Domain.Events.UserProfile;

public sealed record FriendshipCreatedEvent(Guid InitiatorId, Guid IntiateeId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
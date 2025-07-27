using Cypherly.UserManagement.Domain.Abstractions;

namespace Cypherly.UserManagement.Domain.Events.UserProfile;

public sealed record FriendshipCreatedEvent(Guid InitiatorId, Guid IntiateeId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
using Cypherly.UserManagement.Domain.Abstractions;

namespace Cypherly.UserManagement.Domain.Events.UserProfile;

public sealed record UserBlockedEvent(Guid UserProfileId, Guid BlockedUserProfileId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
using Social.Domain.Abstractions;

namespace Social.Domain.Events.UserProfile;

public sealed record UserBlockedEvent(Guid UserProfileId, Guid BlockedUserProfileId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
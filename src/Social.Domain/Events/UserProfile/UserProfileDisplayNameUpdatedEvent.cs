using Social.Domain.Abstractions;

namespace Social.Domain.Events.UserProfile;

public sealed record UserProfileDisplayNameUpdatedEvent(Guid UserProfileId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
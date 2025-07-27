using Cypherly.UserManagement.Domain.Abstractions;

namespace Cypherly.UserManagement.Domain.Events.UserProfile;

public sealed record UserProfileDisplayNameUpdatedEvent(Guid UserProfileId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
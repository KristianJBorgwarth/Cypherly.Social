using Cypherly.UserManagement.Domain.Abstractions;

namespace Cypherly.UserManagement.Domain.Events.UserProfile;

public sealed record FriendshipDeletedEvent(Guid UserProfileId, Guid FriendProfileId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
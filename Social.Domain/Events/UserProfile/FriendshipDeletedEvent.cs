using Social.Domain.Abstractions;

namespace Social.Domain.Events.UserProfile;

public sealed record FriendshipDeletedEvent(Guid UserProfileId, Guid FriendProfileId) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
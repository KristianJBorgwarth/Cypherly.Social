using Social.Domain.Abstractions;
using Social.Domain.Aggregates;
using Social.Domain.Enums;

namespace Social.Domain.Entities;

public class Friendship : Entity
{
    public Guid UserProfileId { get; private set; }
    public Guid FriendProfileId { get; private set; }
    public FriendshipStatus Status { get; private set; }
    public bool IsSeen { get; private set; }
    public virtual UserProfile UserProfile { get; private set; } = null!;
    public virtual UserProfile FriendProfile { get; private set; } = null!;

    public Friendship() { } // Required for EF Core

    public Friendship(Guid userProfileId, Guid friendProfileId) : base(Guid.Empty)
    {
        UserProfileId = userProfileId;
        FriendProfileId = friendProfileId;
        Status = FriendshipStatus.Pending;
    }

    public void AcceptFriendship()
    {
        Status = FriendshipStatus.Accepted;
    }

    public void MarkAsSeen()
    {
        IsSeen = true;
    }
}
using Social.Domain.Abstractions;
using Social.Domain.Aggregates;
using Social.Domain.Enums;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Social.Domain.Entities;

public class Friendship : Entity
{
    public Guid UserProfileId { get; private set; }
    public Guid FriendProfileId { get; private set; }
    public FriendshipStatus Status { get; private set; }
    public bool IsSeen { get; private set; }
    public UserProfile UserProfile { get; private set; }
    public UserProfile FriendProfile { get; private set; }

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
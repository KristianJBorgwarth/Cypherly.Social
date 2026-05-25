using Social.Application.Abstractions;
using Social.Domain.Aggregates;
using Social.Domain.Entities;

namespace Social.Application.Specifications.User;

internal sealed class UserProfileWithFriendRequestsSpec : Specification<UserProfile>
{
    public UserProfileWithFriendRequestsSpec(Guid userId) : base(u => u.Id == userId)
    {
        AddIncludes($"{nameof(UserProfile.FriendshipsReceived)}.{nameof(Friendship.UserProfile)}");
    }
}

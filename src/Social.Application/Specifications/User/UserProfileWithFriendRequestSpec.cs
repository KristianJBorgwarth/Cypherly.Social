using Social.Application.Abstractions;
using Social.Domain.Aggregates;
using Social.Domain.Entities;

namespace Social.Application.Specifications.User;

internal sealed class UserProfileWithFriendRequestsSpec : Specification<UserProfile>
{
    public UserProfileWithFriendRequestsSpec(Guid userId, bool includeAvatar = false) : base(u => u.Id == userId)
    {
        AddIncludes(includeAvatar
            ? $"{nameof(UserProfile.FriendshipsReceived)}.{nameof(Friendship.UserProfile)}.{nameof(UserProfile.Avatar)}"
            : $"{nameof(UserProfile.FriendshipsReceived)}.{nameof(Friendship.UserProfile)}");
    }
}

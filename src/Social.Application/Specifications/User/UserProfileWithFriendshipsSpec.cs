using Social.Application.Abstractions;
using Social.Domain.Aggregates;
using Social.Domain.Entities;

namespace Social.Application.Specifications.User;

internal sealed class UserProfileWithFriendshipsSpec : Specification<UserProfile>
{
    public UserProfileWithFriendshipsSpec(Guid userId) : base(u => u.Id == userId)
    {
        AddIncludes(
            $"{nameof(UserProfile.FriendshipsInitiated)}.{nameof(Friendship.FriendProfile)}.{nameof(UserProfile.Avatar)}",
            $"{nameof(UserProfile.FriendshipsReceived)}.{nameof(Friendship.UserProfile)}.{nameof(UserProfile.Avatar)}"
        );
    }
}

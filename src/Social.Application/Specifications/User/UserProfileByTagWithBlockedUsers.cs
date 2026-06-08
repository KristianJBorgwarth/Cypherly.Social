using Social.Application.Abstractions;
using Social.Domain.Aggregates;
using Social.Domain.Entities;

namespace Social.Application.Specifications.User;

internal sealed class UserProfileByTagWithBlockedUsersSpec : Specification<UserProfile>
{
    public UserProfileByTagWithBlockedUsersSpec(string tag, bool includeFriendships) : base(u => u.UserTag.Tag == tag)
    {
        AddIncludes($"{nameof(UserProfile.BlockedUsers)}.{nameof(BlockedUser.BlockedUserProfile)}");
        if (includeFriendships)
        {
            AddIncludes(
                    $"{nameof(UserProfile.FriendshipsInitiated)}.{nameof(Friendship.FriendProfile)}",
                    $"{nameof(UserProfile.FriendshipsReceived)}.{nameof(Friendship.UserProfile)}"
            );
        }
    }
}

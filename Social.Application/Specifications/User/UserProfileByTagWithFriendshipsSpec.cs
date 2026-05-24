using Social.Application.Abstractions;
using Social.Domain.Aggregates;
using Social.Domain.Entities;

namespace Social.Application.Specifications.User;

internal sealed class UserProfileByTagWithFriendshipsSpec : Specification<UserProfile>
{
    public UserProfileByTagWithFriendshipsSpec(string tag) : base(u => u.UserTag.Tag == tag)
    {
        AddIncludes(
            $"{nameof(UserProfile.FriendshipsInitiated)}.{nameof(Friendship.FriendProfile)}",
            $"{nameof(UserProfile.FriendshipsReceived)}.{nameof(Friendship.UserProfile)}"
        );
    }
}

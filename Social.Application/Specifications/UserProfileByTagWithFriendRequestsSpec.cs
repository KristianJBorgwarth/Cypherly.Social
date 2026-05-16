using Social.Application.Abstractions;
using Social.Domain.Aggregates;
using Social.Domain.Entities;

namespace Social.Application.Specifications;

internal sealed class UserProfileByTagWithFriendRequestsSpec : Specification<UserProfile>
{
    public UserProfileByTagWithFriendRequestsSpec(string tag) : base(u => u.UserTag.Tag == tag)
    {
        AddIncludes($"{nameof(UserProfile.FriendshipsReceived)}.{nameof(Friendship.UserProfile)}");
    }
}
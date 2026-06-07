using Social.Application.Abstractions;
using Social.Domain.Aggregates;
using Social.Domain.Entities;

internal sealed class UserProfileWithAvatarAndFriendsSpec : Specification<UserProfile>
{
    public UserProfileWithAvatarAndFriendsSpec(Guid id) : base(user => user.Id == id)
    {
        AddIncludes($"{nameof(UserProfile.Avatar)}");
        AddIncludes($"{nameof(UserProfile.FriendshipsInitiated)}.{nameof(Friendship.FriendProfile)}");
        AddIncludes($"{nameof(UserProfile.FriendshipsReceived)}.{nameof(Friendship.UserProfile)}");
    }
}


using Social.Application.Abstractions;
using Social.Domain.Aggregates;
using Social.Domain.Entities;

namespace Social.Application.Specifications.Friendships
{
    internal sealed class FriendshipWithUserProfiles : Specification<Friendship>
    {
        public FriendshipWithUserProfiles(
            Guid friendProfileId,
            Guid userProfileId)
            : base(f => (f.FriendProfileId == friendProfileId && f.UserProfileId == userProfileId) ||
                    (f.FriendProfileId == userProfileId && f.UserProfileId == friendProfileId))
        {
            AddIncludes($"{nameof(Friendship.UserProfile)}.{nameof(UserProfile.FriendshipsInitiated)}.{nameof(UserProfile.FriendshipsReceived)}");
            AddIncludes($"{nameof(Friendship.FriendProfile)}.{nameof(UserProfile.FriendshipsReceived)}.{nameof(UserProfile.FriendshipsInitiated)}");
        }
    }
}

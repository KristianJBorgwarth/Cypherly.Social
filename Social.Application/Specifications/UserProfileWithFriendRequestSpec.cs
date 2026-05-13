using Social.Application.Abstractions;
using Social.Domain.Entities;

namespace Social.Application.Specifications;

internal sealed class UserProfileWithFriendRequestsSpec : Specification<Domain.Aggregates.UserProfile>
{
    public UserProfileWithFriendRequestsSpec(Guid userId) : base(u => u.Id == userId)
    {
        AddIncludes($"{nameof(Domain.Aggregates.UserProfile.FriendshipsReceived)}.{nameof(Friendship.UserProfile)}");
    }
}

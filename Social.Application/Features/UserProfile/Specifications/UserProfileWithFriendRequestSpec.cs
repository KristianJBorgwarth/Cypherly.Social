using Social.Application.Abstractions;
using Social.Domain.Entities;

namespace Social.Application.Features.UserProfile.Specifications;

internal sealed class UserProfileWithFriendRequestSpec : Specification<Domain.Aggregates.UserProfile>
{
    public UserProfileWithFriendRequestSpec(Guid userId) : base(u => u.Id == userId)
    {
        AddIncludes($"{nameof(Domain.Aggregates.UserProfile.FriendshipsReceived)}.{nameof(Friendship.UserProfile)}");
    }
}
using Social.Application.Abstractions;
using Social.Domain.Entities;

namespace Social.Application.Features.UserProfile.Specifications;

public sealed class UserProfileWithFriendshipsSpec : Specification<Domain.Aggregates.UserProfile>
{
    public UserProfileWithFriendshipsSpec(Guid userId) : base(u => u.Id == userId)
    {
        AddIncludes(
            $"{nameof(Domain.Aggregates.UserProfile.FriendshipsInitiated)}.{nameof(Friendship.FriendProfile)}",
            $"{nameof(Domain.Aggregates.UserProfile.FriendshipsReceived)}.{nameof(Friendship.UserProfile)}"
        );
    }
}
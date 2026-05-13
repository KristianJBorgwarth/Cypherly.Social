using Social.Application.Abstractions;
using Social.Domain.Entities;

namespace Social.Application.Features.UserProfile.Specifications;

internal sealed class UserProfileByTagWithFriendshipsSpec : Specification<Domain.Aggregates.UserProfile>
{
    public UserProfileByTagWithFriendshipsSpec(string tag) : base(u => u.UserTag.Tag == tag)
    {
        AddIncludes(
            $"{nameof(Domain.Aggregates.UserProfile.FriendshipsInitiated)}.{nameof(Friendship.FriendProfile)}",
            $"{nameof(Domain.Aggregates.UserProfile.FriendshipsReceived)}.{nameof(Friendship.UserProfile)}"
        );
    }
}
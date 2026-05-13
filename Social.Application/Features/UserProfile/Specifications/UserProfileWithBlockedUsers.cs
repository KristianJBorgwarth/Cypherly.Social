using Social.Application.Abstractions;
using Social.Domain.Aggregates;
using Social.Domain.Entities;

internal sealed class UserProfileWithBlockedUsersSpec : Specification<UserProfile>
{
    public UserProfileWithBlockedUsersSpec(Guid userId) : base(u => u.Id == userId)
    {
        AddIncludes($"{nameof(UserProfile.BlockedUsers)}.{nameof(BlockedUser.BlockedUserProfile)}");
    }
}

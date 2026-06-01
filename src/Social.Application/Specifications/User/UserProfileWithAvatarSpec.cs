using Social.Application.Abstractions;
using Social.Domain.Aggregates;

internal sealed class UserProfileWithAvatarSpec : Specification<UserProfile>
{
    public UserProfileWithAvatarSpec(Guid id) : base(user => user.Id == id)
    {
        AddIncludes($"{nameof(UserProfile.Avatar)}");
    }
}

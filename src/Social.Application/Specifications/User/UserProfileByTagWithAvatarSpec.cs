using Social.Application.Abstractions;
using Social.Domain.Aggregates;

internal sealed class UserProfileByTagWithAvatarSpec : Specification<UserProfile>
{
    public UserProfileByTagWithAvatarSpec(string tag) : base(u => u.UserTag.Tag == tag)
    {
        AddIncludes($"{nameof(UserProfile.Avatar)}");
    }
}

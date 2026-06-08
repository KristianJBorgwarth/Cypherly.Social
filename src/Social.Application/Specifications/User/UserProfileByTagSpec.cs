using Social.Application.Abstractions;
using Social.Domain.Aggregates;

internal sealed class UserProfileByTagSpec(string tag) : Specification<UserProfile>(u => u.UserTag.Tag == tag)
{
}

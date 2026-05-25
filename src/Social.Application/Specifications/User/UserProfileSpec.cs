using Social.Application.Abstractions;
using Social.Domain.Aggregates;

namespace Social.Application.Specifications.User
{
    internal sealed class UserProfileSpec(Guid id) : Specification<UserProfile>(user => user.Id == id);
}

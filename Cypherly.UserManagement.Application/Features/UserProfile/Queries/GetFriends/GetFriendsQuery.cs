using Cypherly.Application.Abstractions;
using Cypherly.UserManagement.Application.Abstractions;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetFriends;

public sealed record GetFriendsQuery : IQueryLimited<List<GetFriendsDto>>
{
    public required Guid UserProfileId { get; init; }
}
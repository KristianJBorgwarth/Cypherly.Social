using Cypherly.Application.Abstractions;
using Social.Application.Abstractions;

namespace Social.Application.Features.UserProfile.Queries.GetFriends;

public sealed record GetFriendsQuery : IQueryLimited<List<GetFriendsDto>>
{
    public required Guid UserProfileId { get; init; }
}
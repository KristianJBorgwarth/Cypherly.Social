using Social.Application.Abstractions;

namespace Social.Application.Features.Friendships.Queries.GetFriends;

public sealed record GetFriendsQuery : IQueryLimited<List<GetFriendsDto>>
{
    public required Guid TenantId { get; init; }
}
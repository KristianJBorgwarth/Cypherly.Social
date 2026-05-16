using Social.Application.Abstractions;

namespace Social.Application.Features.Friendships.Queries.GetBlockedUserProfiles;

public sealed record GetBlockedUserProfilesQuery : IQuery<List<GetBlockedUserProfilesDto>>
{
    public required Guid TenantId { get; init; }
}
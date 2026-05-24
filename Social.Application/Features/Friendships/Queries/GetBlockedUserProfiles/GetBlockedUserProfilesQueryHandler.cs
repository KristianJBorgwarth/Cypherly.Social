using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;
using Social.Application.Specifications.User;
using Social.Domain.Aggregates;
using Social.Domain.Common;

namespace Social.Application.Features.Friendships.Queries.GetBlockedUserProfiles;

public sealed class GetBlockedUserProfilesQueryHandler(
    IUserProfileRepository repository,
    ILogger<GetBlockedUserProfilesQueryHandler> logger)
    : IQueryHandler<GetBlockedUserProfilesQuery, List<GetBlockedUserProfilesDto>>
{
    public async Task<Result<List<GetBlockedUserProfilesDto>>> Handle(GetBlockedUserProfilesQuery query, CancellationToken cancellationToken)
    {
        var userProfile = await repository.GetSingleAsync(new UserProfileWithBlockedUsersSpec(query.TenantId), cancellationToken);
        if (userProfile is null)
        {
            logger.LogError("UserProfile with ID: {UserId} not found", query.TenantId);
            return Result.Fail<List<GetBlockedUserProfilesDto>>(Error.NotFound<Social.Domain.Aggregates.UserProfile>(query.TenantId.ToString()));
        }

        var blockedUserProfiles = userProfile.BlockedUsers
            .Select(f => GetBlockedUserProfilesDto.MapFrom(f.BlockedUserProfile))
            .ToList();

        return Result.Ok(blockedUserProfiles);
    }
}

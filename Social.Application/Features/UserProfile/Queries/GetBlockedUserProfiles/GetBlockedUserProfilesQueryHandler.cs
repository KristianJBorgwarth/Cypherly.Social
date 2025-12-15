using Social.Domain.Common;
using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;

namespace Social.Application.Features.UserProfile.Queries.GetBlockedUserProfiles;

public sealed class GetBlockedUserProfilesQueryHandler(
    IUserProfileRepository repository,
    ILogger<GetBlockedUserProfilesQueryHandler> logger)
    : IQueryHandler<GetBlockedUserProfilesQuery, List<GetBlockedUserProfilesDto>>
{
    public async Task<Result<List<GetBlockedUserProfilesDto>>> Handle(GetBlockedUserProfilesQuery query, CancellationToken cancellationToken)
    {
        var userProfile = await repository.GetByIdAsync(query.TenantId);
        if (userProfile is null)
        {
            logger.LogError("UserProfile with ID: {UserId} not found", query.TenantId);
            return Result.Fail<List<GetBlockedUserProfilesDto>>(Errors.General.NotFound(query.TenantId));
        }

        var blockedUserProfiles = userProfile.BlockedUsers
            .Select(f => GetBlockedUserProfilesDto.MapFrom(f.BlockedUserProfile))
            .ToList();

        return Result.Ok(blockedUserProfiles);
    }
}

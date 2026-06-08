using Social.Domain.Common;
using Social.Domain.Interfaces;
using Social.Domain.Services;
using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;
using Social.Application.Specifications.User;

namespace Social.Application.Features.UserProfile.Queries.GetUserProfileByTag;

public class GetUserProfileByTagQueryHandler(
    IUserProfileRepository userProfileRepository,
    IUserBlockingService userBlockingService,
    IFriendshipService friendshipService,
    ILogger<GetUserProfileByTagQueryHandler> logger)
    : IQueryHandler<GetUserProfileByTagQuery, GetUserProfileByTagDto>
{
    public async Task<Result<GetUserProfileByTagDto>> Handle(GetUserProfileByTagQuery q, CancellationToken ct)
    {
        var requestingUser = await userProfileRepository.GetSingleAsync(new UserProfileWithBlockedUsersSpec(q.TenantId, true), ct);
        if (requestingUser is null)
        {
            logger.LogWarning("User with ID: {ID} attempted to get profile by tag: {Tag}, but no user with that ID Exists", q.TenantId, q.Tag);
            return Result.Fail<GetUserProfileByTagDto>(error: Error.NotFound<Domain.Aggregates.UserProfile>(q.TenantId.ToString()));
        }

        var userProfile = await userProfileRepository.GetSingleAsync(new UserProfileByTagWithBlockedUsersSpec(q.Tag, true), ct);

        if (userProfile is null || userBlockingService.IsUserBlocked(requestingUser, userProfile) || userProfile.IsPrivate)
            return Result.Ok<GetUserProfileByTagDto>();

        var friendShipStatusDto = friendshipService.GetFriendshipStatus(requestingUser, userProfile.UserTag.Tag);

        var dto = GetUserProfileByTagDto.MapFrom(userProfile, friendShipStatusDto);

        return Result.Ok(dto);
    }
}

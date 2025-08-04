using Social.Application.Contracts;
using Social.Domain.Common;
using Social.Domain.Interfaces;
using Social.Domain.Services;
using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;
using Social.Application.Contracts.Services;

namespace Social.Application.Features.UserProfile.Queries.GetUserProfileByTag;

public class GetUserProfileByTagQueryHandler(
    IUserProfileRepository userProfileRepository,
    IUserBlockingService userBlockingService,
    IProfilePictureService profilePictureService,
    IFriendshipService friendshipService,
    ILogger<GetUserProfileByTagQueryHandler> logger)
    : IQueryHandler<GetUserProfileByTagQuery, GetUserProfileByTagDto>
{
    public async Task<Result<GetUserProfileByTagDto>> Handle(GetUserProfileByTagQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var requestingUser = await userProfileRepository.GetByIdAsync(request.TenantId);
            if (requestingUser is null)
            {
                logger.LogWarning("User with ID: {ID} attempted to get profile by tag: {Tag}, but no user with that ID Exists", request.TenantId, request.Tag);
                return Result.Fail<GetUserProfileByTagDto>(Errors.General.NotFound(request.TenantId));
            }

            var userProfile = await userProfileRepository.GetByUserTag(request.Tag);

            if (userProfile is null || userBlockingService.IsUserBloccked(requestingUser, userProfile) || userProfile.IsPrivate)
                return Result.Ok<GetUserProfileByTagDto>();

            var friendShipStatusDto = friendshipService.GetFriendshipStatus(requestingUser, userProfile.UserTag.Tag);

            var profilePictureUrl = "";

            if (!string.IsNullOrEmpty(userProfile.ProfilePictureUrl))
            {
                var presignedUrlResult = await profilePictureService.GetPresignedProfilePictureUrlAsync(userProfile.ProfilePictureUrl);
                if (presignedUrlResult.Success is false)
                {
                    logger.LogWarning("Failed to get presigned url for profile picture with key {Key}", userProfile.ProfilePictureUrl);
                }
                else
                {
                    profilePictureUrl = presignedUrlResult.Value;
                }
            }

            var dto = GetUserProfileByTagDto.MapFrom(userProfile, profilePictureUrl, friendShipStatusDto);

            return Result.Ok(dto);
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Exception occured while user with ID: {ID} tried to get profile by tag: {Tag}", request.TenantId, request.Tag);
            return Result.Fail<GetUserProfileByTagDto>(Errors.General.UnspecifiedError("An exception occured while attempting to get the user profile by tag."));
        }
    }
}
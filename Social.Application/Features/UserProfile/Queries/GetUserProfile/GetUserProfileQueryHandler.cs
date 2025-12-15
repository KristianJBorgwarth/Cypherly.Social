using Social.Domain.Common;
using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;
using Social.Application.Contracts.Services;

namespace Social.Application.Features.UserProfile.Queries.GetUserProfile;

public class GetUserProfileQueryHandler(
    IUserProfileRepository userProfileRepository,
    IProfilePictureService profilePictureService,
    ILogger<GetUserProfileQueryHandler> logger)
    : IQueryHandler<GetUserProfileQuery, GetUserProfileDto>
{
    public async Task<Result<GetUserProfileDto>> Handle(GetUserProfileQuery request,
        CancellationToken cancellationToken)
    {
        var userprofile = await userProfileRepository.GetByIdAsync(request.TenantId);
        if (userprofile is null) return Result.Fail<GetUserProfileDto>(Errors.General.NotFound(request.TenantId.ToString()));

        var profilePictureUrl = "";

        if (!string.IsNullOrEmpty(userprofile.ProfilePictureUrl))
        {
            var presignedUrlResult =
                await profilePictureService.GetPresignedProfilePictureUrlAsync(userprofile.ProfilePictureUrl);
            if (presignedUrlResult.Success is false)
            {
                logger.LogWarning("Failed to get presigned url for profile picture with key {Key}",
                    userprofile.ProfilePictureUrl);
            }
            else
            {
                profilePictureUrl = presignedUrlResult.Value;
            }
        }

        var dto = GetUserProfileDto.MapFrom(userprofile, profilePictureUrl);
        return Result.Ok(dto);
    }
}

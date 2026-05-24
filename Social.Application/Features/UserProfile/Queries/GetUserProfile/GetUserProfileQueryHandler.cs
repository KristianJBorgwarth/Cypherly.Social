using Social.Domain.Aggregates;
using Social.Domain.Common;
using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;
using Social.Application.Contracts.Services;
using Social.Application.Specifications.User;

namespace Social.Application.Features.UserProfile.Queries.GetUserProfile;

public class GetUserProfileQueryHandler(
    IUserProfileRepository userProfileRepository,
    IProfilePictureService profilePictureService,
    ILogger<GetUserProfileQueryHandler> logger)
    : IQueryHandler<GetUserProfileQuery, GetUserProfileDto>
{
    public async Task<Result<GetUserProfileDto>> Handle(GetUserProfileQuery q, CancellationToken ct)
    {
        var up = await userProfileRepository.GetSingleAsync(new UserProfileSpec(q.TenantId), ct);
        if (up is null) return Result.Fail<GetUserProfileDto>(Error.NotFound<Social.Domain.Aggregates.UserProfile>(q.TenantId.ToString()));

        var profilePictureUrl = "";

        if (!string.IsNullOrEmpty(up.ProfilePictureUrl))
        {
            var presignedUrlResult = await profilePictureService.GetPresignedProfilePictureUrlAsync(up.ProfilePictureUrl);
            if (!presignedUrlResult.Success)
            {
                logger.LogWarning("Failed to get presigned url for profile picture with key {Key}",
                    up.ProfilePictureUrl);
            }
            else
            {
                profilePictureUrl = presignedUrlResult.Value;
            }
        }

        var dto = GetUserProfileDto.MapFrom(up, profilePictureUrl);
        return Result.Ok(dto);
    }
}

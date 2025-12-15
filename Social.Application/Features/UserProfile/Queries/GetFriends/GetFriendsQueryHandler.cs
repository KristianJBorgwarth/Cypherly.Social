using Social.Domain.Common;
using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;
using Social.Application.Contracts.Services;

// ReSharper disable LoopCanBeConvertedToQuery

namespace Social.Application.Features.UserProfile.Queries.GetFriends;

public class GetFriendsQueryHandler(
    IUserProfileRepository userProfileRepository,
    IProfilePictureService profilePictureService,
    ILogger<GetFriendsQueryHandler> logger)
    : ILimitedQueryHandler<GetFriendsQuery, List<GetFriendsDto>>
{
    public async Task<Result<List<GetFriendsDto>>> Handle(GetFriendsQuery request, CancellationToken cancellationToken)
    {
        var userProfile = await userProfileRepository.GetByIdAsync(request.TenantId);
        if (userProfile is null)
        {
            return Result.Fail<List<GetFriendsDto>>(Errors.General.NotFound(request.TenantId));
        }

        var friends = userProfile.GetFriends();

        if (friends.Count is 0) return Result.Ok(new List<GetFriendsDto>());

        var friendDtos = new List<GetFriendsDto>();

        foreach (var f in friends)
        {
            var presignedUrl = string.Empty;

            if (f.ProfilePictureUrl is not null)
            {
                var presignedUrlResult = await profilePictureService.GetPresignedProfilePictureUrlAsync(f.ProfilePictureUrl);
                if (presignedUrlResult.Success)
                {
                    presignedUrl = presignedUrlResult.Value;
                }
                else
                {
                    logger.LogWarning("Failed to get presigned URL for profile picture of user {UserId}", f.Id);
                }
            }

            friendDtos.Add(new GetFriendsDto(userProfile: f, presignedUrl: presignedUrl));
        }

        return Result.Ok(friendDtos);
    }
}

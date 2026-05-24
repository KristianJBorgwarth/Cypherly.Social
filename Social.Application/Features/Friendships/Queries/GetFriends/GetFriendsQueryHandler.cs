using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;
using Social.Application.Contracts.Services;
using Social.Application.Specifications.User;
using Social.Domain.Aggregates;
using Social.Domain.Common;

// ReSharper disable LoopCanBeConvertedToQuery

namespace Social.Application.Features.Friendships.Queries.GetFriends;

public class GetFriendsQueryHandler(
    IUserProfileRepository userProfileRepository,
    IProfilePictureService profilePictureService,
    ILogger<GetFriendsQueryHandler> logger)
    : ILimitedQueryHandler<GetFriendsQuery, List<GetFriendsDto>>
{
    public async Task<Result<List<GetFriendsDto>>> Handle(GetFriendsQuery q, CancellationToken ct)
    {
        var userProfile = await userProfileRepository.GetSingleAsync(new UserProfileWithFriendshipsSpec(q.TenantId), ct);
        if (userProfile is null)
        {
            return Result.Fail<List<GetFriendsDto>>(Error.NotFound<Social.Domain.Aggregates.UserProfile>(q.TenantId.ToString()));
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

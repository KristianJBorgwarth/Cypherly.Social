using Cypherly.Application.Abstractions;
using Social.Application.Contracts;
using Cypherly.UserManagement.Domain.Common;
using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Clients;
using Social.Application.Contracts.Repositories;
using Social.Application.Contracts.Services;

// ReSharper disable LoopCanBeConvertedToQuery

namespace Social.Application.Features.UserProfile.Queries.GetFriends;

public class GetFriendsQueryHandler(
    IUserProfileRepository userProfileRepository,
    IConnectionIdProvider connectionIdProvider,
    IProfilePictureService profilePictureService,
    ILogger<GetFriendsQueryHandler> logger)
    : ILimitedQueryHandler<GetFriendsQuery, List<GetFriendsDto>>
{
    public async Task<Result<List<GetFriendsDto>>> Handle(GetFriendsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userProfile = await userProfileRepository.GetByIdAsync(request.UserProfileId);
            if (userProfile is null)
            {
                return Result.Fail<List<GetFriendsDto>>(Errors.General.NotFound(request.UserProfileId));
            }

            var friends = userProfile.GetFriends();

            if (friends.Count is 0) return Result.Ok(new List<GetFriendsDto>());

            var allConnectionIds = await connectionIdProvider.GetConnectionIdsByUsers(friends.Select(f => f.Id).ToArray());

            var friendDtos = new List<GetFriendsDto>();

            foreach (var f in friends)
            {
                var connectionIds = allConnectionIds[f.Id];
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

                friendDtos.Add(GetFriendsDto.MapFrom(f, connectionIds, presignedUrl));
            }

            return Result.Ok(friendDtos);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occurred in {Handler}, while attempting to retrieve friends for {UserProfileId}",
                nameof(GetFriendsQueryHandler), request.UserProfileId);
            return Result.Fail<List<GetFriendsDto>>(Errors.General.UnspecifiedError("An exception occurred while attempting to retrieve friends."));
        }
    }
}
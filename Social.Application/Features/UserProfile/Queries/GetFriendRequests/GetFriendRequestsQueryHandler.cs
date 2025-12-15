using Social.Domain.Common;
using Social.Domain.Enums;
using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;
using Social.Application.Contracts.Services;

namespace Social.Application.Features.UserProfile.Queries.GetFriendRequests;

public class GetFriendRequestsQueryHandler(
    IUserProfileRepository userProfileRepository,
    IProfilePictureService profilePictureService,
    ILogger<GetFriendRequestsQueryHandler> logger)
    : IQueryHandler<GetFriendRequestsQuery, GetFriendRequestsDto[]>
{
    public async Task<Result<GetFriendRequestsDto[]>> Handle(GetFriendRequestsQuery query, CancellationToken cancellationToken)
    {
        var userProfile = await userProfileRepository.GetByIdAsync(query.TenantId);
        if (userProfile is null)
        {
            logger.LogCritical("UserProfile not found, UserProfileId: {UserProfileId}", query.TenantId);
            return Result.Fail<GetFriendRequestsDto[]>(Errors.General.NotFound(query.TenantId));
        }

        var friendRequests = userProfile.FriendshipsReceived
            .Where(f => f.Status == FriendshipStatus.Pending)
            .ToList();

        var friendRequestDtos = friendRequests.Select(async f =>
        {

            if (f.UserProfile.ProfilePictureUrl is null)
                return GetFriendRequestsDto.MapFrom(
                    f.UserProfile.Username,
                    f.UserProfile.UserTag.Tag,
                    f.UserProfile.DisplayName);

            var profilePictureUrl = await GetProfilePictureUrl(f.UserProfile.ProfilePictureUrl);

            return GetFriendRequestsDto.MapFrom(
                f.UserProfile.Username,
                f.UserProfile.UserTag.Tag,
                f.UserProfile.DisplayName,
                profilePictureUrl);

        }).ToArray();

        var friendRequestDtosList = await Task.WhenAll(friendRequestDtos);

        return Result.Ok(friendRequestDtosList);

    }

    private async Task<string?> GetProfilePictureUrl(string? profilePictureUrl)
    {
        if (profilePictureUrl is null)
            return null;

        var result = await profilePictureService.GetPresignedProfilePictureUrlAsync(profilePictureUrl);
        if (result.Success)
            return result.Value;

        logger.LogWarning("Failed to get presigned URL for profile picture, UserProfileId: {UserProfileId}, Error: {Error}", profilePictureUrl, result.Error);
        return null;
    }
}

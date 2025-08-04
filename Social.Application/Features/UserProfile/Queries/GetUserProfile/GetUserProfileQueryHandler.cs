using Social.Application.Contracts;
using Social.Domain.Common;
using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Clients;
using Social.Application.Contracts.Repositories;
using Social.Application.Contracts.Services;

namespace Social.Application.Features.UserProfile.Queries.GetUserProfile;

public class GetUserProfileQueryHandler(
    IUserProfileRepository userProfileRepository,
    IProfilePictureService profilePictureService,
    IConnectionIdProvider connectionIdProvider,
    ILogger<GetUserProfileQueryHandler> logger)
    : IQueryHandler<GetUserProfileQuery, GetUserProfileDto>
{
    public async Task<Result<GetUserProfileDto>> Handle(GetUserProfileQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userprofile = await userProfileRepository.GetByIdAsync(request.TenantId);
            if (userprofile is null) return Result.Fail<GetUserProfileDto>(Errors.General.NotFound(request.TenantId.ToString()));

            var connectionIds = await GetConnectionIdsAsync(request.TenantId, request.ExclusiveConnectionId, cancellationToken);

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

            var dto = GetUserProfileDto.MapFrom(userprofile, profilePictureUrl, connectionIds);
            return Result.Ok(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occured while handling request with id {Id}", request.TenantId);
            return Result.Fail<GetUserProfileDto>(
                Errors.General.UnspecifiedError("An exception occured while handling the request"));
        }
    }

    private async Task<Guid[]> GetConnectionIdsAsync(Guid userProfileId, Guid exclusiveConnectionId, CancellationToken cancellationToken)
    {
        var connectionIds = await connectionIdProvider.GetConnectionIdsByUser(userProfileId, cancellationToken);

        var filteredConnectionIds = connectionIds
            .Where(id => id != exclusiveConnectionId)
            .ToArray();

        return filteredConnectionIds;
    }
}
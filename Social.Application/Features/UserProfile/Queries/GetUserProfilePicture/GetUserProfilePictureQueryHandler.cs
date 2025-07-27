using Cypherly.Application.Abstractions;
using Social.Application.Contracts;
using Social.Domain.Common;
using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Clients;

namespace Social.Application.Features.UserProfile.Queries.GetUserProfilePicture;

public sealed class GetUserProfilePictureQueryHandler(
    IMinioProxyClient minioProxyClient,
    ILogger<GetUserProfilePictureQueryValidator> logger)
    : IQueryHandler<GetUserProfilePictureQuery, GetUserProfilePictureDto>
{
    public async Task<Result<GetUserProfilePictureDto>> Handle(GetUserProfilePictureQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var data = await minioProxyClient.GetImageFromMinioAsync(request.ProfilePictureUrl, cancellationToken);

            if (data.HasValue is false)
            {
                return Result.Fail<GetUserProfilePictureDto>(Errors.General.UnspecifiedError("Error getting user profile picture"));
            }

            var dto = GetUserProfilePictureDto.MapFrom(data.Value.image, data.Value.imageType);
            return Result.Ok(dto);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting user profile picture");
            return Result.Fail<GetUserProfilePictureDto>(Errors.General.UnspecifiedError("Exception occured while getting user profile picture"));
        }
    }
}
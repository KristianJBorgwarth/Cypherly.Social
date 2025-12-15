using Social.Domain.Common;
using Social.Application.Abstractions;
using Social.Application.Contracts.Clients;

namespace Social.Application.Features.UserProfile.Queries.GetUserProfilePicture;

public sealed class GetUserProfilePictureQueryHandler(IMinioProxyClient minioProxyClient) : IQueryHandler<GetUserProfilePictureQuery, GetUserProfilePictureDto>
{
    public async Task<Result<GetUserProfilePictureDto>> Handle(GetUserProfilePictureQuery request, CancellationToken cancellationToken)
    {
        var data = await minioProxyClient.GetImageFromMinioAsync(request.ProfilePictureUrl, cancellationToken);

        if (data.HasValue is false)
        {
            return Result.Fail<GetUserProfilePictureDto>(Errors.General.UnspecifiedError("Error getting user profile picture"));
        }

        var dto = GetUserProfilePictureDto.MapFrom(data.Value.image, data.Value.imageType);
        return Result.Ok(dto);
    }
}

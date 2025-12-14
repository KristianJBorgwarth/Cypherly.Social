using Social.Domain.Common;
using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;
using Social.Application.Contracts.Services;

namespace Social.Application.Features.UserProfile.Commands.Update.ProfilePicture;

public class UpdateUserProfilePictureCommandHandler(
    IUserProfileRepository userProfileRepository,
    IProfilePictureService profilePictureService,
    IUnitOfWork unitOfWork,
    ILogger<UpdateUserProfilePictureCommandHandler> logger)
    : ICommandHandler<UpdateUserProfilePictureCommand, UpdateUserProfilePictureDto>
{
    public async Task<Result<UpdateUserProfilePictureDto>> Handle(UpdateUserProfilePictureCommand request, CancellationToken cancellationToken)
    {
        var user = await userProfileRepository.GetByIdAsync(request.TenantId);
        if (user is null) return Result.Fail<UpdateUserProfilePictureDto>(Errors.General.NotFound(request.TenantId));

        var result = await profilePictureService.UploadProfilePictureAsync(request.NewProfilePicture, request.TenantId);
        if (result.Success is false) return Result.Fail<UpdateUserProfilePictureDto>(result.Error);

        user.SetProfilePictureUrl(result.Value);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        string? presignedUrl = null;

        var presignedUrlResult = await profilePictureService.GetPresignedProfilePictureUrlAsync(result.Value);

        if (presignedUrlResult.Success is false)
        {
            logger.LogWarning("Failed to retrieve presigned URL for profile picture with key {Key} for user {UserId}",
                result.Value, request.TenantId);
        }
        else
        {
            presignedUrl = presignedUrlResult.Value;
        }

        var dto = new UpdateUserProfilePictureDto(presignedUrl);

        return Result.Ok(dto);
    }
}

using Social.Application.Contracts;
using Social.Domain.Common;
using Social.Domain.Interfaces;
using Social.Domain.Services;
using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Clients;
using Social.Application.Contracts.Repositories;
using Social.Application.Contracts.Services;

namespace Social.Application.Features.UserProfile.Commands.Update.AcceptFriendship;

public class AcceptFriendshipCommandHandler(
    IUserProfileRepository userProfileRepository,
    IFriendshipService friendshipService,
    IProfilePictureService profilePictureService,
    IConnectionIdProvider connectionIdProvider,
    IUnitOfWork unitOfWork,
    ILogger<AcceptFriendshipCommandHandler> logger)
    : ICommandHandler<AcceptFriendshipCommand, AcceptFriendshipDto>
{
    public async Task<Result<AcceptFriendshipDto>> Handle(AcceptFriendshipCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userProfile = await userProfileRepository.GetByIdAsync(request.TenantId);
            if (userProfile is null)
            {
                logger.LogError("User not found: {UserId}", request.TenantId);
                return Result.Fail<AcceptFriendshipDto>(Errors.General.NotFound(request.TenantId));
            }

            var result = friendshipService.AcceptFriendship(userProfile, request.FriendTag);
            if (result.Success is false)
            {
                logger.LogError("Error accepting friendship: {Error} {UserId} {FriendTag}", result.Error, request.TenantId, request.FriendTag);
                return Result.Fail<AcceptFriendshipDto>(result.Error);
            }
            var newFriend = await userProfileRepository.GetByUserTag(request.FriendTag);

            if (newFriend is null)
            {
                logger.LogError("Friend not found: {FriendTag}", request.FriendTag);
                return Result.Fail<AcceptFriendshipDto>(Errors.General.NotFound(request.FriendTag));
            }

            var connectionIds = await connectionIdProvider.GetConnectionIdsSingleTenant(newFriend.Id, cancellationToken);

            await userProfileRepository.UpdateAsync(userProfile);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var presignedUrl = await FetchPresignedUrl(newFriend.ProfilePictureUrl);

            var acceptFriendshipDto = AcceptFriendshipDto.MapFrom(newFriend, presignedUrl, connectionIds);

            return Result.Ok(acceptFriendshipDto);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error accepting friendship: {UserId} {FriendTag}", request.TenantId, request.FriendTag);
            return Result.Fail<AcceptFriendshipDto>(Errors.General.UnspecifiedError("An exception occured while accepting friendship"));
        }
    }

    private async Task<string?> FetchPresignedUrl(string? profilePictureUrl)
    {
        if (string.IsNullOrEmpty(profilePictureUrl))
            return string.Empty;

        var presignedUrl = await profilePictureService.GetPresignedProfilePictureUrlAsync(profilePictureUrl);
        return presignedUrl;
    }
}
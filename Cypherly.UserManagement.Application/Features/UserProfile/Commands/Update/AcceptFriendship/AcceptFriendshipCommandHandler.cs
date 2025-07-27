using Cypherly.Application.Abstractions;
using Cypherly.UserManagement.Application.Abstractions;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Application.Contracts.Clients;
using Cypherly.UserManagement.Application.Contracts.Repositories;
using Cypherly.UserManagement.Application.Contracts.Services;
using Cypherly.UserManagement.Domain.Common;
using Cypherly.UserManagement.Domain.Interfaces;
using Cypherly.UserManagement.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.AcceptFriendship;

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
            var userProfile = await userProfileRepository.GetByIdAsync(request.Id);
            if (userProfile is null)
            {
                logger.LogError("User not found: {UserId}", request.Id);
                return Result.Fail<AcceptFriendshipDto>(Errors.General.NotFound(request.Id));
            }

            var result = friendshipService.AcceptFriendship(userProfile, request.FriendTag);
            if (result.Success is false)
            {
                logger.LogError("Error accepting friendship: {Error} {UserId} {FriendTag}", result.Error, request.Id, request.FriendTag);
                return Result.Fail<AcceptFriendshipDto>(result.Error);
            }
            var newFriend = await userProfileRepository.GetByUserTag(request.FriendTag);

            if (newFriend is null)
            {
                logger.LogError("Friend not found: {FriendTag}", request.FriendTag);
                return Result.Fail<AcceptFriendshipDto>(Errors.General.NotFound(request.FriendTag));
            }

            var connectionIds = await connectionIdProvider.GetConnectionIdsByUser(newFriend.Id, cancellationToken);

            await userProfileRepository.UpdateAsync(userProfile);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var presignedUrl = await FetchPresignedUrl(newFriend.ProfilePictureUrl);

            var acceptFriendshipDto = AcceptFriendshipDto.MapFrom(newFriend, presignedUrl, connectionIds);

            return Result.Ok(acceptFriendshipDto);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error accepting friendship: {UserId} {FriendTag}", request.Id, request.FriendTag);
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
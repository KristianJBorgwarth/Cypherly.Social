using Social.Domain.Common;
using Social.Domain.Services;
using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;
using Social.Application.Features.UserProfile.Specifications;

namespace Social.Application.Features.UserProfile.Commands.Update.BlockUser;

public class BlockUserCommandHandler(
    IUserProfileRepository userProfileRepository,
    IUserBlockingService userBlockingService,
    IUnitOfWork uow,
    ILogger<BlockUserCommandHandler> logger)
    : ICommandHandler<BlockUserCommand>
{
    public async Task<Result> Handle(BlockUserCommand request, CancellationToken cancellationToken)
    {
        var userProfile = await userProfileRepository.GetSingleAsync(new UserProfileWithFriendshipsSpec(request.TenantId), cancellationToken);
        if (userProfile is null)
        {
            logger.LogError("User profile not found for user id {UserId}", request.TenantId);
            return Result.Fail(Errors.General.NotFound(request.TenantId));
        }

        var blockedUserProfile = await userProfileRepository.GetSingleAsync(new UserProfileByTagWithFriendshipsSpec(request.BlockedUserTag), cancellationToken);
        if (blockedUserProfile is null)
        {
            logger.LogError("User profile not found for user id {UserId}", request.BlockedUserTag);
            return Result.Fail(Errors.General.NotFound(request.BlockedUserTag));
        }

        userBlockingService.BlockUser(userProfile, blockedUserProfile);
        await uow.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}

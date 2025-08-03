using Social.Application.Contracts;
using Social.Domain.Common;
using Social.Domain.Services;
using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;

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
        try
        {
            var userProfile = await userProfileRepository.GetByIdAsync(request.TenantId);
            if (userProfile is null)
            {
                logger.LogError("User profile not found for user id {UserId}", request.TenantId);
                return Result.Fail(Errors.General.NotFound(request.TenantId));
            }

            var blockedUserProfile = await userProfileRepository.GetByUserTag(request.BlockedUserTag);
            if (blockedUserProfile is null)
            {
                logger.LogError("User profile not found for user id {UserId}", request.BlockedUserTag);
                return Result.Fail(Errors.General.NotFound(request.BlockedUserTag));
            }

            userBlockingService.BlockUser(userProfile, blockedUserProfile);
            await uow.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error blocking user {UserId} for user {BlockedUserTag}", request.TenantId,
                request.BlockedUserTag);
            return Result.Fail(Errors.General.UnspecifiedError(
                "Exception occured whilte attempting to block user. Please Check logs for more details"));
        }
    }
}
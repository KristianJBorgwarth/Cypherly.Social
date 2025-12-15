using Social.Domain.Common;
using Social.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;

namespace Social.Application.Features.UserProfile.Commands.Delete.Friendship;

public sealed class DeleteFriendshipCommandHandler(
    IUserProfileRepository profileRepository,
    IUnitOfWork unitOfWork,
    IFriendshipService friendshipService,
    ILogger<DeleteFriendshipCommandHandler> logger)
    : ICommandHandler<DeleteFriendshipCommand>
{
    public async Task<Result> Handle(DeleteFriendshipCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling DeleteFriendshipCommand for UserProfileId {UserProfileId} and FriendTag {FriendTag}", request.TenantId, request.FriendTag);
        var userProfile = await profileRepository.GetByIdAsync(request.TenantId);
        if (userProfile is null)
        {
            logger.LogError("UserProfile with id {UserProfileId} not found", request.TenantId);
            return Result.Fail(Errors.General.NotFound(nameof(request.TenantId)));
        }
        var deleteResult = friendshipService.DeleteFriendship(userProfile, request.FriendTag);
        if (!deleteResult.Success)
        {
            logger.LogError("Failed to delete friendship with FriendTag {FriendTag} for UserProfileId {UserProfileId}", request.FriendTag, request.TenantId);
            return Result.Fail(deleteResult.Error);
        }

        await profileRepository.UpdateAsync(userProfile);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}

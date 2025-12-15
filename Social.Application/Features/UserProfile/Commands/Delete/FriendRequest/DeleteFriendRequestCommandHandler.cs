using Social.Domain.Common;
using Social.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;

namespace Social.Application.Features.UserProfile.Commands.Delete.FriendRequest;

public sealed class DeleteFriendRequestCommandHandler(
    IUserProfileRepository userProfileRepository,
    IUnitOfWork unitOfWork,
    IFriendshipService friendshipService,
    ILogger<DeleteFriendRequestCommandHandler> logger
    ) : ICommandHandler<DeleteFriendRequestCommand>
{
    public async Task<Result> Handle(DeleteFriendRequestCommand request, CancellationToken cancellationToken)
    {
        var userProfile = await userProfileRepository.GetByIdAsync(request.TenantId);
        if (userProfile is null)
        {
            logger.LogWarning("User profile with ID {Id} not found.", request.TenantId);
            return Result.Fail(Errors.General.NotFound(request.TenantId));
        }

        var delResult = friendshipService.DeleteFriendRequest(userProfile, request.FriendTag);

        if (delResult.Success is false)
        {
            return delResult;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}

using Social.Application.Contracts;
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
        try
        {
            var userProfile = await userProfileRepository.GetByIdAsync(request.Id);
            if (userProfile is null)
            {
                logger.LogWarning("User profile with ID {Id} not found.", request.Id);
                return Result.Fail(Errors.General.NotFound(request.Id));
            }

            var delResult = friendshipService.DeleteFriendRequest(userProfile, request.FriendTag);

            if (delResult.Success is false)
            {
                return delResult;
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while deleting friend request for user profile with ID {Id}.", request.Id);
            return Result.Fail(Errors.General.UnspecifiedError("An error occurred while processing your request."));
        }
    }
}
using Cypherly.Application.Abstractions;
using Cypherly.UserManagement.Application.Abstractions;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Application.Contracts.Repositories;
using Cypherly.UserManagement.Domain.Common;
using Cypherly.UserManagement.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.MarkFriendRequestAsSeen;

public class MarkFriendRequestsReadCommandHandler(
    IUserProfileRepository userProfileRepository,
    IFriendshipService friendshipService,
    IUnitOfWork unitOfWork,
    ILogger<MarkFriendRequestsReadCommandHandler> logger)
    : ICommandHandler<MarkFriendRequestsReadCommand>
{
    public async Task<Result> Handle(MarkFriendRequestsReadCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userProfile = await userProfileRepository.GetByIdAsync(request.Id);
            if (userProfile is null)
            {
                logger.LogError("User profile not found for ID: {Id}", request.Id);
                return Result.Fail(Errors.General.NotFound(request.Id));
            }

            friendshipService.MarkeFriendshipAsSeen(userProfile, request.RequestTags.ToArray());

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error marking friend requests as seen for user profile ID: {Id}", request.Id);
            return Result.Fail(Errors.General.UnspecifiedError("An error occurred while marking friend requests as seen"));
        }
    }
}
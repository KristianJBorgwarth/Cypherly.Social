using Social.Domain.Common;
using Social.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;

namespace Social.Application.Features.UserProfile.Commands.Update.MarkFriendRequestAsSeen;

public class MarkFriendRequestsAsSeendCommandHandler(
    IUserProfileRepository userProfileRepository,
    IFriendshipService friendshipService,
    IUnitOfWork unitOfWork,
    ILogger<MarkFriendRequestsAsSeendCommandHandler> logger)
    : ICommandHandler<MarkFriendRequestsAsSeenCommand>
{
    public async Task<Result> Handle(MarkFriendRequestsAsSeenCommand request, CancellationToken cancellationToken)
    {
        var userProfile = await userProfileRepository.GetByIdAsync(request.TenantId);
        if (userProfile is null)
        {
            logger.LogError("User profile not found for ID: {Id}", request.TenantId);
            return Result.Fail(Errors.General.NotFound(request.TenantId));
        }

        friendshipService.MarkeFriendshipAsSeen(userProfile, [.. request.RequestTags]);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}

using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;
using Social.Application.Specifications.User;
using Social.Domain.Aggregates;
using Social.Domain.Common;
using Social.Domain.Interfaces;

namespace Social.Application.Features.Friendships.Commands.Delete.FriendRequest;

public sealed class DeleteFriendRequestCommandHandler(
    IUserProfileRepository userProfileRepository,
    IUnitOfWork unitOfWork,
    IFriendshipService friendshipService,
    ILogger<DeleteFriendRequestCommandHandler> logger
    ) : ICommandHandler<DeleteFriendRequestCommand>
{
    public async Task<Result> Handle(DeleteFriendRequestCommand request, CancellationToken ct)
    {
        var userProfile = await userProfileRepository.GetSingleAsync(new UserProfileWithFriendRequestsSpec(request.TenantId), ct);
        if (userProfile is null)
        {
            logger.LogWarning("User profile with ID {Id} not found.", request.TenantId);
            return Result.Fail(Error.NotFound<Social.Domain.Aggregates.UserProfile>(request.TenantId.ToString()));
        }

        var delResult = friendshipService.DeleteFriendRequest(userProfile, request.FriendTag);

        if (delResult.Success is false)
        {
            return delResult;
        }

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Ok();
    }
}

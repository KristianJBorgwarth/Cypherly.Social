using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;
using Social.Application.Specifications.User;
using Social.Domain.Aggregates;
using Social.Domain.Common;
using Social.Domain.Interfaces;

namespace Social.Application.Features.Friendships.Commands.Update.MarkFriendRequestAsSeen;

public class MarkFriendRequestsAsSeenCommandHandler(
    IUserProfileRepository userProfileRepository,
    IFriendshipService friendshipService,
    IUnitOfWork unitOfWork,
    ILogger<MarkFriendRequestsAsSeenCommandHandler> logger)
    : ICommandHandler<MarkFriendRequestsAsSeenCommand>
{
    public async Task<Result> Handle(MarkFriendRequestsAsSeenCommand q, CancellationToken ct)
    {
        var userProfile = await userProfileRepository.GetSingleAsync(new UserProfileWithFriendRequestsSpec(q.TenantId), ct);
        if (userProfile is null)
        {
            logger.LogError("User profile not found for ID: {Id}", q.TenantId);
            return Result.Fail(Error.NotFound<Social.Domain.Aggregates.UserProfile>(q.TenantId.ToString()));
        }

        friendshipService.MarkeFriendshipAsSeen(userProfile, [.. q.RequestTags]);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Ok();
    }
}

using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;
using Social.Application.Specifications.User;
using Social.Domain.Common;
using Social.Domain.Interfaces;
using Social.Domain.Services;

namespace Social.Application.Features.Friendships.Commands.Create;

public class CreateFriendshipCommandHandler(
    IUserProfileRepository userProfileRepository,
    IFriendshipService friendshipService,
    IUserBlockingService userBlockingService,
    IUnitOfWork unitOfWork,
    ILogger<CreateFriendshipCommandHandler> logger)
    : ICommandHandler<CreateFriendshipCommand>
{
    public async ValueTask<Result> Handle(CreateFriendshipCommand cmd, CancellationToken ct)
    {
        var friend = await userProfileRepository.GetSingleAsync(new UserProfileByTagWithBlockedUsersSpec(cmd.FriendTag, includeFriendships: true), ct);
        if (friend is null)
        {
            logger.LogWarning("Friend not found for {FriendTag}", cmd.FriendTag);
            return Result.Fail(Error.NotFound<Domain.Aggregates.UserProfile>(cmd.FriendTag));
        }

        var userProfile = await userProfileRepository.GetSingleAsync(new UserProfileWithBlockedUsersSpec(cmd.TenantId, includeFriendships: true), ct);
        if (userProfile is null)
        {
            logger.LogWarning("User not found for {UserId}", cmd.TenantId);
            return Result.Fail(Error.NotFound<Domain.Aggregates.UserProfile>(cmd.TenantId.ToString()));
        }

        // Private and blocked profiles answer exactly like a missing one, so the response doesn't reveal they exist.
        if (friend.IsPrivate || userBlockingService.IsUserBlocked(userProfile, friend))
        {
            logger.LogWarning("Friend request from {UserId} to {FriendTag} refused: profile is private or blocked", cmd.TenantId, cmd.FriendTag);
            return Result.Fail(Error.NotFound<Domain.Aggregates.UserProfile>(cmd.FriendTag));
        }

        var result = friendshipService.CreateFriendship(userProfile, friend);
        if (result.Success is false)
        {
            logger.LogWarning("Error creating friendship between {UserId} and {FriendId}", cmd.TenantId, friend.Id);
            return Result.Fail(result.Error);
        }

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Ok();
    }
}

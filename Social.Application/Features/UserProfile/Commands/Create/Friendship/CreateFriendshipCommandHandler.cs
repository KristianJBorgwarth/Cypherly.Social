using Social.Domain.Common;
using Social.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;
using Social.Application.Features.UserProfile.Specifications;

namespace Social.Application.Features.UserProfile.Commands.Create.Friendship;

public class CreateFriendshipCommandHandler(
    IUserProfileRepository userProfileRepository,
    IFriendshipService friendshipService,
    IUnitOfWork unitOfWork,
    ILogger<CreateFriendshipCommandHandler> logger)
    : ICommandHandler<CreateFriendshipCommand>
{
    public async Task<Result> Handle(CreateFriendshipCommand cmd, CancellationToken ct)
    {
        var friend = await userProfileRepository.GetSingleAsync(new UserProfileByTagWithFriendshipsSpec(cmd.FriendTag), ct);
        if (friend is null)
        {
            logger.LogWarning("Friend not found for {FriendTag}", cmd.FriendTag);
            return Result.Fail(Errors.General.NotFound(cmd.FriendTag));
        }

        var userProfile = await userProfileRepository.GetSingleAsync(new UserProfileWithFriendshipsSpec(cmd.TenantId), ct);
        if (userProfile is null)
        {
            logger.LogWarning("User not found for {UserId}", cmd.TenantId);
            return Result.Fail(Errors.General.NotFound(cmd.TenantId));
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

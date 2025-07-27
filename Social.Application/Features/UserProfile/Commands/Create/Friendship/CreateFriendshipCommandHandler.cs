using Cypherly.Application.Abstractions;
using Social.Application.Contracts;
using Social.Domain.Common;
using Social.Domain.Interfaces;
using Social.Domain.Services;
using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;

namespace Social.Application.Features.UserProfile.Commands.Create.Friendship;

public class CreateFriendshipCommandHandler(
    IUserProfileRepository userProfileRepository,
    IFriendshipService friendshipService,
    IUnitOfWork unitOfWork,
    ILogger<CreateFriendshipCommandHandler> logger)
    : ICommandHandler<CreateFriendshipCommand>
{
    public async Task<Result> Handle(CreateFriendshipCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var friend = await userProfileRepository.GetByUserTag(request.FriendTag);
            if (friend is null)
            {
                logger.LogWarning("Friend not found for {FriendTag}", request.FriendTag);
                return Result.Fail(Errors.General.NotFound(request.FriendTag));
            }

            var userProfile = await userProfileRepository.GetByIdAsync(request.Id);
            if (userProfile is null)
            {
                logger.LogWarning("User not found for {UserId}", request.Id);
                return Result.Fail(Errors.General.NotFound(request.Id));
            }

            var result = friendshipService.CreateFriendship(userProfile, friend);
            if (result.Success is false)
            {
                logger.LogWarning("Error creating friendship between {UserId} and {FriendId}", request.Id, friend.Id);
                return Result.Fail(result.Error);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in {Command}", nameof(CreateFriendshipCommand));
            return Result.Fail(Errors.General.UnspecifiedError(ex.Message));
        }
    }
}
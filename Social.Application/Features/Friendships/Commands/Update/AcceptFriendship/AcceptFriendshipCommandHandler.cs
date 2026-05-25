using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Clients;
using Social.Application.Contracts.Repositories;
using Social.Application.Specifications.User;
using Social.Domain.Common;
using Social.Domain.Interfaces;

namespace Social.Application.Features.Friendships.Commands.Update.AcceptFriendship;

public class AcceptFriendshipCommandHandler(
    IUserProfileRepository userProfileRepository,
    IFriendshipService friendshipService,
    IConnectionIdProvider connectionIdProvider,
    IUnitOfWork unitOfWork,
    ILogger<AcceptFriendshipCommandHandler> logger)
    : ICommandHandler<AcceptFriendshipCommand, AcceptFriendshipDto>
{
    public async Task<Result<AcceptFriendshipDto>> Handle(AcceptFriendshipCommand cmd, CancellationToken ct)
    {
        var userProfile = await userProfileRepository.GetSingleAsync(new UserProfileWithFriendshipsSpec(cmd.TenantId), ct);
        if (userProfile is null)
        {
            logger.LogError("User not found: {UserId}", cmd.TenantId);
            return Result.Fail<AcceptFriendshipDto>(Error.NotFound<Domain.Aggregates.UserProfile>(cmd.TenantId.ToString()));
        }

        var result = friendshipService.AcceptFriendship(userProfile, cmd.FriendTag);
        if (!result.Success)
        {
            logger.LogError("Error accepting friendship: {Error} {UserId} {FriendTag}", result.Error, cmd.TenantId, cmd.FriendTag);
            return Result.Fail<AcceptFriendshipDto>(result.Error);
        }
        var newFriend = await userProfileRepository.GetSingleAsync(new UserProfileByTagWithFriendshipsSpec(cmd.FriendTag), ct);

        if (newFriend is null)
        {
            logger.LogError("Friend not found: {FriendTag}", cmd.FriendTag);
            return Result.Fail<AcceptFriendshipDto>(Error.NotFound<Domain.Aggregates.UserProfile>(cmd.FriendTag));
        }

        var connectionIds = await connectionIdProvider.GetConnectionIdsSingleTenant(newFriend.Id, ct);

        await userProfileRepository.UpdateAsync(userProfile, ct);
        await unitOfWork.SaveChangesAsync(ct);


        var acceptFriendshipDto = AcceptFriendshipDto.MapFrom(newFriend, connectionIds);

        return Result.Ok(acceptFriendshipDto);
    }
}

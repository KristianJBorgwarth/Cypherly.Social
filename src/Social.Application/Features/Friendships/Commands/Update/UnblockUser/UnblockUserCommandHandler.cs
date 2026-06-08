using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;
using Social.Application.Specifications.User;
using Social.Domain.Common;
using Social.Domain.Services;

namespace Social.Application.Features.Friendships.Commands.Update.UnblockUser;

public class UnblockUserCommandHandler(
    IUserProfileRepository userProfileRepository,
    IUnitOfWork unitOfWork,
    IUserBlockingService userBlockingService,
    ILogger<UnblockUserCommandHandler> logger)
    : ICommandHandler<UnblockUserCommand>
{
    public async Task<Result> Handle(UnblockUserCommand cmd, CancellationToken ct)
    {
        var userProfile = await userProfileRepository.GetSingleAsync(new UserProfileWithBlockedUsersSpec(cmd.TenantId), ct);
        if (userProfile is null)
        {
            logger.LogCritical("User with {ID} not found", cmd.TenantId);
            return Result.Fail(Error.NotFound<Domain.Aggregates.UserProfile>(cmd.TenantId.ToString()));
        }

        var userToUnblock = await userProfileRepository.GetSingleAsync(new UserProfileByTagWithBlockedUsersSpec(cmd.Tag), ct);
        if (userToUnblock is null)
        {
            logger.LogCritical("User with tag {Tag} not found", cmd.Tag);
            return Result.Fail(Error.NotFound<Domain.Aggregates.UserProfile>(cmd.Tag));
        }

        userBlockingService.UnblockUser(userProfile, userToUnblock);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Ok();
    }
}

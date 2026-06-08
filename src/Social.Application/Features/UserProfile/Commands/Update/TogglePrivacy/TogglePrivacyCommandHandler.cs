using Social.Domain.Common;
using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;
using Social.Application.Specifications.User;

namespace Social.Application.Features.UserProfile.Commands.Update.TogglePrivacy;

public class TogglePrivacyCommandHandler(
    IUserProfileRepository userProfileRepository,
    IUnitOfWork unitOfWork,
    ILogger<TogglePrivacyCommandHandler> logger)
    : ICommandHandler<TogglePrivacyCommand>
{
    public async Task<Result> Handle(TogglePrivacyCommand cmd, CancellationToken ct)
    {
        var userProfile = await userProfileRepository.GetSingleAsync(new UserProfileSpec(cmd.TenantId), ct);
        if (userProfile is null)
        {
            logger.LogCritical("User with {ID} not found", cmd.TenantId);
            return Result.Fail(Error.NotFound<Domain.Aggregates.UserProfile>(cmd.TenantId.ToString()));
        }

        userProfile.TogglePrivacy(cmd.IsPrivate);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Ok();
    }
}

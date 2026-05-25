using Social.Domain.Common;
using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;

namespace Social.Application.Features.UserProfile.Commands.Update.DisplayName;

public class UpdateUserProfileDisplayNameCommandHandler(
    IUserProfileRepository userProfileRepository,
    ILogger<UpdateUserProfileDisplayNameCommandHandler> logger,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateUserProfileDisplayNameCommand, UpdateUserProfileDisplayNameDto>
{
    public async Task<Result<UpdateUserProfileDisplayNameDto>> Handle(UpdateUserProfileDisplayNameCommand cmd, CancellationToken ct)
    {
        var userProfile = await userProfileRepository.GetByIdAsync(cmd.TenantId, ct);
        if (userProfile is null)
        {
            logger.LogWarning("User profile with id: {Id} not found.", cmd.TenantId);
            return Result.Fail<UpdateUserProfileDisplayNameDto>(Error.NotFound<Social.Domain.Aggregates.UserProfile>(cmd.TenantId.ToString()));
        }

        var result = userProfile.SetDisplayName(cmd.DisplayName);
        if (!result.Success) return Result.Fail<UpdateUserProfileDisplayNameDto>(result.Error);

        await userProfileRepository.UpdateAsync(userProfile, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Ok(new UpdateUserProfileDisplayNameDto(){DisplayName = userProfile.DisplayName!});
    }
}

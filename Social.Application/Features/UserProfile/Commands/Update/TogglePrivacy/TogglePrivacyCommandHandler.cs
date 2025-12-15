using Social.Domain.Common;
using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;

namespace Social.Application.Features.UserProfile.Commands.Update.TogglePrivacy;

public class TogglePrivacyCommandHandler(
    IUserProfileRepository userProfileRepository,
    IUnitOfWork unitOfWork,
    ILogger<TogglePrivacyCommandHandler> logger)
    : ICommandHandler<TogglePrivacyCommand>
{
    public async Task<Result> Handle(TogglePrivacyCommand request, CancellationToken cancellationToken)
    {
        var userProfile = await userProfileRepository.GetByIdAsync(request.TenantId);
        if (userProfile is null)
        {
            logger.LogCritical("User with {ID} not found", request.TenantId);
            return Result.Fail(Errors.General.NotFound(request.TenantId));
        }

        userProfile.TogglePrivacy(request.IsPrivate);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}

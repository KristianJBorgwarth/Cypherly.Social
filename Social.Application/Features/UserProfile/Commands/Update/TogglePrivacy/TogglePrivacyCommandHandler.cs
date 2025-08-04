using Social.Application.Contracts;
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
        try
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
        catch (Exception e)
        {
            logger.LogCritical(e, "An exception occurred while toggling privacy for user with {ID}", request.TenantId);
            return Result.Fail(Errors.General.UnspecifiedError("An exception occurred while attempting to toggle privacy"));
        }
    }
}
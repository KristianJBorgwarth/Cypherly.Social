using AutoMapper;
using Social.Domain.Common;
using Microsoft.Extensions.Logging;
using Social.Application.Abstractions;
using Social.Application.Contracts.Repositories;

namespace Social.Application.Features.UserProfile.Commands.Update.DisplayName;

public class UpdateUserProfileDisplayNameCommandHandler(
    IUserProfileRepository userProfileRepository,
    IMapper mapper,
    ILogger<UpdateUserProfileDisplayNameCommandHandler> logger,
    IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateUserProfileDisplayNameCommand, UpdateUserProfileDisplayNameDto>
{
    public async Task<Result<UpdateUserProfileDisplayNameDto>> Handle(UpdateUserProfileDisplayNameCommand request, CancellationToken cancellationToken)
    {
        var userProfile = await userProfileRepository.GetByIdAsync(request.TenantId);
        if (userProfile is null)
        {
            logger.LogWarning("User profile with id: {Id} not found.", request.TenantId);
            return Result.Fail<UpdateUserProfileDisplayNameDto>(Errors.General.NotFound("User profile not found."));
        }

        var result = userProfile.SetDisplayName(request.DisplayName);
        if (result.Success is false) return Result.Fail<UpdateUserProfileDisplayNameDto>(result.Error);

        await userProfileRepository.UpdateAsync(userProfile);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = mapper.Map<UpdateUserProfileDisplayNameDto>(userProfile);
        return Result.Ok(dto);
    }
}
